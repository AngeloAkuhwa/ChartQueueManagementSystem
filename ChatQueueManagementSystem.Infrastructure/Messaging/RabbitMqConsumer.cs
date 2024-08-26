using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Application.Services;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Domain.Enums;
using ChatQueueManagementSystem.Infrastructure.Settings;
using ChatQueueManagementSystem.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using ChatQueueManagementSystem.Application.Common.Helpers;

namespace ChatQueueManagementSystem.Infrastructure.Messaging
{
	public class RabbitMqConsumer : BackgroundService
	{
		private readonly IModel _channel;
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<RabbitMqConsumer> _logger;
		private readonly RabbitMqSettings _rabbitMqSettings;
		private string _queueName = string.Empty;
		private readonly OfficeHours _officeHours;
		private int _currentAgentIndex;
		private readonly object _lock = new object();
		private static ConcurrentDictionary<string, List<Agent>> _agentsCache = new ConcurrentDictionary<string, List<Agent>>();

		public RabbitMqConsumer(IModel channel, IServiceProvider serviceProvider, ILogger<RabbitMqConsumer> logger, IOptions<OfficeHours> officeHoursOptions, IOptions<RabbitMqSettings> rabbitMqSettings)
		{
			_channel = channel;
			_serviceProvider = serviceProvider;
			_logger = logger;
			_officeHours = officeHoursOptions.Value;
			_currentAgentIndex = 0;
			_rabbitMqSettings = rabbitMqSettings.Value;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += async (model, ea) =>
			{
				await HandleMessageAsync(ea, stoppingToken);
			};

			_channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

			await Task.CompletedTask;
		}

		private async Task HandleMessageAsync(BasicDeliverEventArgs ea, CancellationToken stoppingToken)
		{
			try
			{
				var message = Encoding.UTF8.GetString(ea.Body.ToArray());
				var data = JsonConvert.DeserializeObject<dynamic>(message);
				_logger.LogInformation("Received message: {Message}", message);

				if (data == null)
				{
					_logger.LogWarning("Received null data.");
					_channel.BasicAck(ea.DeliveryTag, false);
					return;
				}

				var chatSession = data.ChatSession.ToObject<ChatSession>();
				if (chatSession == null)
				{
					_logger.LogWarning("Received null ChatSession.");
					_channel.BasicAck(ea.DeliveryTag, false);
					return;
				}

				_queueName = data.QueueName;
				using var scope = _serviceProvider.CreateScope();
				var agentRepository = scope.ServiceProvider.GetRequiredService<IAgentRepository>();
				var chatSessionRepository = scope.ServiceProvider.GetRequiredService<IChatSessionRepository>();
				var chatQueueService = scope.ServiceProvider.GetRequiredService<IChatQueueService>();
				var queueRepository = scope.ServiceProvider.GetRequiredService<IQueueRepository>();
				var chatQueueContext = scope.ServiceProvider.GetRequiredService<ChatQueueDbContext>();

				var agents = await GetAvailableAgentsAsync(agentRepository);
				var currentQueue = await queueRepository.GetQueueByNameAsync(_queueName);

				if (!agents.Any() || currentQueue == null)
				{
					_logger.LogWarning("No available agents or queue found.");
					_channel.BasicAck(ea.DeliveryTag, false);
					return;
				}

				lock (_lock)
				{
					var nextAvailableAgent = agents[_currentAgentIndex % agents.Count];
					_currentAgentIndex++;

					if (nextAvailableAgent.CurrentConcurrentChats >= ChatsHelper.CalculateCurrentChatCapacity(new List<Agent>{nextAvailableAgent}))
					{
						RepublishChatSession(chatSession);
						_channel.BasicAck(ea.DeliveryTag, false);
						return;
					}

					ProcessChatSession(chatSession, nextAvailableAgent, currentQueue, chatQueueService, queueRepository, chatSessionRepository, chatQueueContext, stoppingToken).GetAwaiter().GetResult();
				}

				_channel.BasicAck(ea.DeliveryTag, false);
			}
			catch (Exception ex)
			{
				var data = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(ea.Body.ToArray()));

				_logger.LogError(ex, $"Unexpected error processing message.\n  dataReceived: {data}");
				_channel.BasicReject(ea.DeliveryTag, false);
			}
		}

		private async Task<List<Agent>> GetAvailableAgentsAsync(IAgentRepository agentRepository)
		{
			if (!_agentsCache.TryGetValue("agents", out var agents))
			{
				agents = (await agentRepository.GetAvailableAgentsAsync()).ToList();
				var sortedAgents = agents
					.Where(a => a.IsWithinShift(DateTime.UtcNow) && a.CurrentConcurrentChats <
						ChatsHelper.CalculateCurrentChatCapacity(new List<Agent> { a })).OrderBy(x => x.CreatedAt).ToList();

				_agentsCache["agents"] = sortedAgents;
			}

			return agents;
		}

		private void RepublishChatSession(ChatSession chatSession)
		{
			var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
			{
				ChatSession = chatSession,
				QueueName = _queueName
			}));

			_channel.BasicPublish(exchange: _rabbitMqSettings.ExchangeName, routingKey: _rabbitMqSettings.RoutingKey, basicProperties: null, body: body);
			_logger.LogInformation("Requeued chat session: {ChatSessionId}", chatSession.Id);
		}

		private async Task ProcessChatSession(ChatSession chatSession, Agent nextAvailableAgent, Queue currentQueue, IChatQueueService chatQueueService, IQueueRepository queueRepository, IChatSessionRepository chatSessionRepository, ChatQueueDbContext chatQueueContext, CancellationToken stoppingToken)
		{
			var chatQueueIsFull = await chatQueueService.IsQueueFullAsync(nextAvailableAgent.TeamId, currentQueue.Id);
			var overFlowQueue = await queueRepository.GetQueueByTypeAsync(true);
			var chatWithinOfficeHours = chatSession.StartTime >= _officeHours.WorkDayStart && chatSession.StartTime <= _officeHours.WorkDayEnd;

			if (chatQueueIsFull)
			{
				await HandleFullQueueAsync(chatSession, nextAvailableAgent, chatWithinOfficeHours, overFlowQueue, currentQueue, chatQueueContext, chatSessionRepository, stoppingToken);
			}
			else
			{
				AssignAgentToChatSession(chatSession, nextAvailableAgent);
				await chatSessionRepository.UpdateAsync(chatSession);
				await chatQueueContext.SaveChangesAsync(stoppingToken);
				_logger.LogInformation("Chat session updated successfully.");
			}
		}

		private async Task HandleFullQueueAsync(ChatSession chatSession, Agent nextAvailableAgent, bool chatWithinOfficeHours, Queue? overFlowQueue, Queue currentQueue, ChatQueueDbContext chatQueueContext, IChatSessionRepository chatSessionRepository, CancellationToken stoppingToken)
		{
			if (chatWithinOfficeHours && overFlowQueue != null)
			{
				_logger.LogInformation("Chat is within office hours and overflow queue is available.");
				currentQueue = overFlowQueue;
				var overflow = new Overflow
				{
					Id = Guid.NewGuid(),
					Agents = new List<Agent> { nextAvailableAgent }
				};

				_queueName = overFlowQueue.QueueName;
				await chatQueueContext.Overflows.AddAsync(overflow, stoppingToken);
				//await chatSessionRepository.AssignAgentToSessionAsync(chatSession.Id, nextAvailableAgent.Id);
				AssignAgentToChatSession(chatSession, nextAvailableAgent);
				await chatSessionRepository.UpdateAsync(chatSession);

				_logger.LogInformation("Overflow chat session updated successfully.");
			}
			else
			{
				_logger.LogInformation("Queue with QueueId: {QueueId} is full and no overflow queue available.", currentQueue.Id);

				AssignAgentToChatSession(chatSession, nextAvailableAgent, true);
				await chatSessionRepository.UpdateAsync(chatSession);

				_logger.LogInformation("ChatSession with chatSessionId: {ChatSessionId} is refused.", chatSession.Id);
			}
		}

		private void AssignAgentToChatSession(ChatSession chatSession, Agent nextAvailableAgent, bool isRefused = false)
		{
			chatSession.AgentId = nextAvailableAgent.Id;
			chatSession.Status = isRefused ? ChatStatus.Refused : ChatStatus.Active;
			chatSession.IsActive = true;
			chatSession.StartTime = DateTime.UtcNow;
			nextAvailableAgent.CurrentConcurrentChats++;
		}

		public override void Dispose()
		{
			_channel?.Close();
			base.Dispose();
		}
	}
}
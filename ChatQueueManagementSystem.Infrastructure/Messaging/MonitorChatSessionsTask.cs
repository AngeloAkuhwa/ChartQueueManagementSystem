using ChatQueueManagementSystem.Application.Common.Interfaces.Messaging;
using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChatQueueManagementSystem.Infrastructure.Messaging
{
	public class MonitorChatSessionsTask : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<MonitorChatSessionsTask> _logger;
		private Timer _timer;

		public MonitorChatSessionsTask(IServiceProvider serviceProvider, ILogger<MonitorChatSessionsTask> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_timer = new Timer(MonitorChatSessions, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
			return Task.CompletedTask;
		}

		private async void MonitorChatSessions(object state)
		{
			_logger.LogInformation("Monitoring chat sessions...");
			using var scope = _serviceProvider.CreateScope();
			var chatSessionRepository = scope.ServiceProvider.GetRequiredService<IChatSessionRepository>();
			var queueRepository = scope.ServiceProvider.GetRequiredService<IQueueRepository>();
			var agentRepository = scope.ServiceProvider.GetRequiredService<IAgentRepository>();
			var rabbitMqProducer = scope.ServiceProvider.GetRequiredService<IRabbitMqProducer>();

			var activeSessions = await chatSessionRepository.GetActiveSessionsAsync();
			foreach (var session in activeSessions)
			{
				if (!session.IsActive || session.Status == ChatStatus.Completed)
					continue;

				session.UpdateStatusOnPoll(false);

				if (session.Status == ChatStatus.Refused)
				{
					await HandleSessionRefusal(session, queueRepository, chatSessionRepository, rabbitMqProducer);
				}
				else
				{
					if (await ShouldMarkAsCompleted(session, chatSessionRepository, agentRepository))
					{
						_logger.LogInformation($"Chat session {session.Id} marked as completed.");
					}

					await chatSessionRepository.UpdateAsync(session);
					_logger.LogInformation($"Chat session {session.Id} status updated to {session.Status}.");
				}
			}
		}

		private async Task<bool> ShouldMarkAsCompleted(ChatSession chatSession, IChatSessionRepository chatSessionRepository, IAgentRepository agentRepository)
		{
			if (chatSession.Status != ChatStatus.Active) return false;

			var prevCurrentConcurrentChats = 0;
			var updatedConCurrentChat = 0;

			if (chatSession.IsActive && DateTime.UtcNow - chatSession.StartTime > TimeSpan.FromHours(2)) //simulating time out scene
			{
				chatSession.EndTime = DateTime.UtcNow;
				chatSession.Status = ChatStatus.Completed;
				chatSession.IsActive = false;

				_logger.LogInformation($"Chat session {chatSession.Id} completed.");

				await chatSessionRepository.UpdateAsync(chatSession);

				if (chatSession.AgentId.HasValue)
				{
					var agent = await agentRepository.GetByIdAsync(chatSession.AgentId.Value);
					if (agent != null)
					{
						prevCurrentConcurrentChats = agent.CurrentConcurrentChats;
						updatedConCurrentChat = agent.CurrentConcurrentChats = Math.Max(0, agent.CurrentConcurrentChats - 1);
						await agentRepository.UpdateAsync(agent);
						await chatSessionRepository.UpdateAsync(chatSession);

						_logger.LogInformation($"Updated concurrent chats for agent {agent.Id}.");
					}
				}
			}

			return updatedConCurrentChat < prevCurrentConcurrentChats;
		}

		private async Task HandleSessionRefusal(ChatSession session, IQueueRepository queueRepository, IChatSessionRepository chatSessionRepository, IRabbitMqProducer rabbitMqProducer)
		{
			if(session.QueueId == null)
			{
				_logger.LogError($"Chat session {session.Id} does not have a queue ID.");
				return;
			}

			var queue = await queueRepository.GetByIdAsync(session.QueueId.Value);

			if (queue == null)
			{
				_logger.LogError($"Queue with ID {session.QueueId.Value} not found.");
				return;
			}

			if (!queue.IsOverflow || !IsWithinOfficeHours())
			{
				session.IsActive = false;
				session.Status = ChatStatus.Refused;
				_logger.LogInformation($"Chat session {session.Id} is refused due to out of office hours.");
			}
			else
			{
				var overflowQueue = await queueRepository.GetQueueByTypeAsync(true);
				if (overflowQueue != null)
				{
					session.QueueId = overflowQueue.Id;
					session.Status = ChatStatus.Queued;
					session.IsActive = true;
					_logger.LogInformation($"Chat session {session.Id} moved to overflow queue.");
					RepublishSessionToQueue(session, rabbitMqProducer, overflowQueue.QueueName);
				}
			}

			await chatSessionRepository.UpdateAsync(session);
		}

		private static void RepublishSessionToQueue(ChatSession chatSession, IRabbitMqProducer rabbitMqProducer, string queueName)
		{
			var message = new
			{
				ChatSession = chatSession,
				QueueName = queueName
			};

			rabbitMqProducer.PublishMessage(JsonConvert.SerializeObject(message));
		}

		private bool IsWithinOfficeHours()
		{
			var now = DateTime.UtcNow.TimeOfDay;
			return now >= TimeSpan.FromHours(8) && now <= TimeSpan.FromHours(17);
		}

		public override Task StopAsync(CancellationToken stoppingToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		public override void Dispose()
		{
			_timer?.Dispose();
			base.Dispose();
		}
	}
}

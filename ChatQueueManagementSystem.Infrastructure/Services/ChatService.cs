using ChatQueueManagementSystem.Application.Common.Interfaces.Messaging;
using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Application.Services;
using ChatQueueManagementSystem.Domain.Entities;
using Newtonsoft.Json;

namespace ChatQueueManagementSystem.Infrastructure.Services
{
	public class ChatService : IChatService
	{
		private readonly IChatSessionRepository _chatSessionRepository;
		private readonly IQueueRepository _queueRepository;
		private readonly IRabbitMqProducer _rabbitMqProducer;

		public ChatService(IChatSessionRepository chatSessionRepository, IRabbitMqProducer rabbitMqProducer, IQueueRepository queueRepository)
		{
			_chatSessionRepository = chatSessionRepository;
			_rabbitMqProducer = rabbitMqProducer;
			_queueRepository = queueRepository;
		}

		public async Task CreateChatSessionAsync(ChatSession chatSession, string queueName)
		{
			await _chatSessionRepository.AddAsync(chatSession);
			await _queueRepository.AddChatSessionToQueueAsync(chatSession.QueueId!.Value, chatSession);

			var message = new
			{
				ChatSession = chatSession,
				QueueName = queueName
			};

			_rabbitMqProducer.PublishMessage(JsonConvert.SerializeObject(message));
		}
	}
}
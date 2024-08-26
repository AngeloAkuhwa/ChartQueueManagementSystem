using ChatQueueManagementSystem.Domain.Entities;

namespace ChatQueueManagementSystem.Application.Services
{
	public interface IChatService
	{
		Task CreateChatSessionAsync(ChatSession chatSession, string queueName);
	}
}

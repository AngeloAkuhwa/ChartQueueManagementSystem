using ChatQueueManagementSystem.Domain.Entities;

namespace ChatQueueManagementSystem.Application.Common.Interfaces.Repositories
{
	public interface IQueueRepository : IGenericRepository<Queue>
	{
		Task<Queue?> GetQueueByTypeAsync(bool isOverflow);
		Task<Queue?> GetQueueByNameAsync(string queueName);
		Task<bool> AddChatSessionToQueueAsync(Guid queueId, ChatSession chatSession);
		Task<bool> RemoveChatSessionFromQueueAsync(Guid queueId, Guid chatSessionId);
	}
}
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Domain.Enums;

namespace ChatQueueManagementSystem.Application.Common.Interfaces.Repositories
{
	public interface IChatSessionRepository : IGenericRepository<ChatSession>
	{
		Task<IEnumerable<ChatSession>> GetByStatusAsync(ChatStatus status);
		Task<bool> AssignAgentToSessionAsync(Guid chatSessionId, Guid agentId, bool isRefused = false);
		Task<IEnumerable<ChatSession>> GetActiveSessionsAsync();
	}
}
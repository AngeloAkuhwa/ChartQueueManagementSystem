using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Domain.Enums;
using ChatQueueManagementSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatQueueManagementSystem.Infrastructure.Repositories
{
	public class ChatSessionRepository : GenericRepository<ChatSession>, IChatSessionRepository
	{
		public ChatSessionRepository(ChatQueueDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<ChatSession>> GetByStatusAsync(ChatStatus status)
		{
			return await Context.ChatSessions.Where(cs => cs.Status == status).ToListAsync();
		}

		public async Task<bool> AssignAgentToSessionAsync(Guid chatSessionId, Guid agentId, bool isRefused = false)
		{
			var session = await Context.ChatSessions.FirstOrDefaultAsync(x => x.Id == chatSessionId);
			var agent = await Context.Agents.FirstOrDefaultAsync(x => x.Id == agentId);

			if (session == null || agent == null) return false;

			session.AgentId = agentId;
			session.Status = ChatStatus.Active;
			session.IsActive = true;
			session.StartTime = DateTime.UtcNow;

			return await Context.SaveChangesAsync() > 0;
		}

		public async Task<IEnumerable<ChatSession>> GetActiveSessionsAsync()
		{
			return await Context.ChatSessions.Where(cs => cs.IsActive).ToListAsync();
		}
	}
}

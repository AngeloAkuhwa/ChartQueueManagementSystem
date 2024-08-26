using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatQueueManagementSystem.Infrastructure.Repositories
{
	public class AgentRepository : GenericRepository<Agent>, IAgentRepository
	{
		public AgentRepository(ChatQueueDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Agent>> GetAvailableAgentsAsync()
		{
			return await Context.Agents.Where(a => a.CurrentConcurrentChats < a.MaxConcurrentChats && a.IsWithinShift(DateTime.UtcNow)).ToListAsync();
		}

		public async Task<bool> UpdateAgentLoadAsync(Guid agentId)
		{
			var agent = await Context.Agents.FirstOrDefaultAsync(a => a.Id == agentId);
			if (agent == null) return false;

			agent.CurrentConcurrentChats += 1;
			await SaveChangesAsync();
			return true;
		}
	}
}
using ChatQueueManagementSystem.Domain.Entities;

namespace ChatQueueManagementSystem.Application.Common.Interfaces.Repositories
{
	public interface IAgentRepository : IGenericRepository<Agent>
	{
		Task<IEnumerable<Agent>> GetAvailableAgentsAsync();
		Task<bool> UpdateAgentLoadAsync(Guid agentId);
	}
}
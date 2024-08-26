using ChatQueueManagementSystem.Domain.Entities;

namespace ChatQueueManagementSystem.Application.Common.Interfaces.Repositories
{
	public interface ITeamRepository : IGenericRepository<Team>
	{
		Task<Team?> GetTeamByNameAsync(string name);
		Task<IEnumerable<Team>> GetAllTeamsWithAgentsAsync();
		Task<Team?> GetTeamByIdAsync(Guid teamId);
	}
}
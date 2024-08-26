using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatQueueManagementSystem.Infrastructure.Repositories
{
	public class TeamRepository : GenericRepository<Team>, ITeamRepository
	{
		public TeamRepository(ChatQueueDbContext context) : base(context)
		{
		}

		public async Task<Team?> GetTeamByNameAsync(string name)
		{
			return await Context.Teams.FirstOrDefaultAsync(t => t.Name == name);
		}

		public async Task<IEnumerable<Team>> GetAllTeamsWithAgentsAsync()
		{
			return await Context.Teams.Include(t => t.Agents).ToListAsync();
		}

		public async Task<Team?> GetTeamByIdAsync(Guid teamId)
		{
			return await Context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);

		}
	}
}

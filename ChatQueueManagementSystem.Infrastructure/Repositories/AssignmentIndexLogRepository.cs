using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatQueueManagementSystem.Infrastructure.Repositories
{
    internal class AssignmentIndexLogRepository : GenericRepository<AssignmentIndexLogVersion>, IAssignmentIndexLogRepository
	{
		public AssignmentIndexLogRepository(ChatQueueDbContext context) : base(context)
		{
		}

		public async Task<AssignmentIndexLogVersion?> GetLatestIndexLogAsync()
		{
			return await Context.AssignmentIndexLogVersions.OrderByDescending(a => a.CreatedAt).FirstOrDefaultAsync();
		}
	}
}

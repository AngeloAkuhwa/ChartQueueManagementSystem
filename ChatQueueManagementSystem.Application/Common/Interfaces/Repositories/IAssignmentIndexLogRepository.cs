using ChatQueueManagementSystem.Domain.Entities;

namespace ChatQueueManagementSystem.Application.Common.Interfaces.Repositories
{
    public interface IAssignmentIndexLogRepository	: IGenericRepository<AssignmentIndexLogVersion>
	{
		Task<AssignmentIndexLogVersion?> GetLatestIndexLogAsync();
	}
}

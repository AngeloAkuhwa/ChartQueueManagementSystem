namespace ChatQueueManagementSystem.Application.Services
{
	public interface IChatQueueService
	{
		Task<bool> IsQueueFullAsync(Guid teamId, Guid queueId);
	}
}

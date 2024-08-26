using ChatQueueManagementSystem.Application.Common.Helpers;
using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Application.Services;

namespace ChatQueueManagementSystem.Infrastructure.Services
{
	public class ChatQueueService : IChatQueueService
	{
		private readonly IQueueRepository _queueRepository;
		private readonly ITeamRepository _teamRepository;

		public ChatQueueService(ITeamRepository teamRepository, IQueueRepository queueRepository)
		{
			_teamRepository = teamRepository;
			_queueRepository = queueRepository;
		}

		public async Task<bool> IsQueueFullAsync(Guid teamId, Guid queueId)
		{
			var team = await _teamRepository.GetByIdAsync(teamId);
			var queue = await _queueRepository.GetByIdAsync(queueId);

			if (team is null || queue is null)
			{
				return true;
			}

			var maxQueueLength = ChatsHelper.CalculateMaximumQueueLength(team.Agents);
			return queue.QueueLength >= maxQueueLength;
		}
	}
}
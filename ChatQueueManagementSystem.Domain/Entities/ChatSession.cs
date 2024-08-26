using ChatQueueManagementSystem.Domain.Entities.Base;
using ChatQueueManagementSystem.Domain.Enums;

namespace ChatQueueManagementSystem.Domain.Entities
{
	public sealed class ChatSession: BaseEntity
	{
		public Guid? QueueId { get; set; }
		public Guid UserId { get; set; }
		public Guid? AgentId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public ChatStatus Status { get; set; }
		public bool IsActive { get; set; }
		public int InactivityCounter { get; set; }
		public string Message { get; set; }

		private const int MaxMissedPolls = 3;

		public void UpdateStatusOnPoll(bool isPolling)
		{
			if (Status != ChatStatus.Active)
			{
				return;
			}

			if (isPolling)
			{
				InactivityCounter = 0;
			}
			else
			{
				InactivityCounter++;

				if (InactivityCounter >= MaxMissedPolls)
				{
					// Mark the session as refused due to inactivity
					Status = ChatStatus.Refused;
					IsActive = false;
					EndTime = DateTime.UtcNow;
				}
			}
		}
	}
}
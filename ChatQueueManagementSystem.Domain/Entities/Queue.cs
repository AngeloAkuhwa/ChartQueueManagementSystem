using ChatQueueManagementSystem.Domain.Entities.Base;

namespace ChatQueueManagementSystem.Domain.Entities
{
	public sealed class Queue: BaseEntity
	{
		public string QueueName { get; set; }
		public List<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
		public int QueueLength { get; set; }
		public bool IsOverflow { get; set; }
	}
}
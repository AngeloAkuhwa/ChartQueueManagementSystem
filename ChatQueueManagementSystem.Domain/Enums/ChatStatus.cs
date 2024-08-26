namespace ChatQueueManagementSystem.Domain.Enums
{
	public enum ChatStatus
	{
		Queued, // Chat is in the queue
		Active, // Chat is currently active
		Completed, // Chat is completed
		Refused // Chat is refused
	}
}
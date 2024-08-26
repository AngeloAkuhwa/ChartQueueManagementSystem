using ChatQueueManagementSystem.Domain.Entities.Base;

namespace ChatQueueManagementSystem.Domain.Entities
{
	public sealed class User: BaseEntity
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
	}
}
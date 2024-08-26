using ChatQueueManagementSystem.Domain.Entities.Base;

namespace ChatQueueManagementSystem.Domain.Entities
{
	public sealed class Overflow: BaseEntity
	{
		public List<Agent> Agents { get; set; } = new List<Agent>();
	}
}
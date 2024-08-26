using ChatQueueManagementSystem.Domain.Entities.Base;
using ChatQueueManagementSystem.Domain.Enums;

namespace ChatQueueManagementSystem.Domain.Entities
{
	public sealed class Team: BaseEntity
	{
		public string Name { get; set; }
		public List<Agent> Agents { get; set; } = new List<Agent>();
	}
}
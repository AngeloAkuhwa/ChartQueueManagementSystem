using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Domain.Enums;

namespace ChatQueueManagementSystem.Application.Common.Helpers
{
	public class ChatsHelper
	{
		public static int CalculateCurrentChatCapacity(List<Agent> agents)
		{
			return agents.Sum(agent => (int)(agent.SeniorityMultiplier * 10 * (int)agent.SeniorityLevel));
		}

		public static int CalculateMaximumQueueLength(List<Agent> agents) => (int)(CalculateCurrentChatCapacity(agents) * 1.5);
	}
}

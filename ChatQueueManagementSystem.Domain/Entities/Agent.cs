using ChatQueueManagementSystem.Domain.Entities.Base;
using ChatQueueManagementSystem.Domain.Enums;

namespace ChatQueueManagementSystem.Domain.Entities
{
	public sealed class Agent: BaseEntity
	{
		public Guid TeamId { get; set; }
		public string Name { get; set; }
		public SeniorityLevel SeniorityLevel { get; set; }
		public int MaxConcurrentChats { get; set; }
		public int CurrentConcurrentChats { get; set; }
		public double SeniorityMultiplier { get; set; }
		public TimeSpan ShiftStartTime { get; set; }
		public TimeSpan ShiftDuration { get; set; } = TimeSpan.FromHours(8);

		public bool IsWithinShift(DateTime currentTime)
		{
			var shiftEndTime = ShiftStartTime.Add(ShiftDuration);
			var currentHour = currentTime.TimeOfDay;
			return currentHour >= ShiftStartTime && currentHour <= shiftEndTime;
		}
	}
}
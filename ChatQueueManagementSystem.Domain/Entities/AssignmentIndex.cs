using ChatQueueManagementSystem.Domain.Entities.Base;
using ChatQueueManagementSystem.Domain.Enums;

namespace ChatQueueManagementSystem.Domain.Entities
{
    public sealed class AssignmentIndexLogVersion : BaseEntity
    {
        public Guid AgentId { get; set; }
        public Guid ChatSessionId { get; set; }
        public ChatStatus ChatStatus { get; set; }
        public string QueueName { get; set; }
        public int CurrentAgentIndex { get; set; }
    }
}
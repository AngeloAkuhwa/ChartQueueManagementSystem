using ChatQueueManagementSystem.Domain.Entities;
using static ChatQueueManagementSystem.Application.Features.ChatSession.Queries.GetChatSessionStatus;

namespace ChatQueueManagementSystem.Application.Common.Helpers.Mapper
{
	public static class RequestChatSessionMapper
	{
		public static ChatSessionStatusResult GetResponse(this ChatSession chatSession)
		{
			return new ChatSessionStatusResult()
			{
				SessionId = chatSession.Id,
				AgentId = chatSession.AgentId,
				UserId = chatSession.UserId,
				QueueId = chatSession.QueueId,
				Status = chatSession.Status,
				Message = chatSession.Message,
				IsActive = chatSession.IsActive,
				StartTime = chatSession.StartTime,
				EndTime = chatSession.EndTime
			};
		}
	}
}
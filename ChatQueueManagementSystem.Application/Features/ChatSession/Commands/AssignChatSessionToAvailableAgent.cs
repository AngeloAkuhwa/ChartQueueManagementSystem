using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Domain.Enums;
using MediatR;
using System.Net;

namespace ChatQueueManagementSystem.Application.Features.ChatSession.Commands
{
	public static class AssignChatSessionToAvailableAgent
	{
		public class Command : IRequest<AssignChatSessionResult>
		{
			public Guid SessionId { get; set; }
			public Guid AgentId { get; set; }
		}

		public class AssignChatSessionResult
		{
			public HttpStatusCode StatusCode { get; }
			public string Message { get; }

			public AssignChatSessionResult(HttpStatusCode statusCode, string message)
			{
				StatusCode = statusCode;
				Message = message;
			}

			public AssignChatSessionResult(string message) : this(HttpStatusCode.OK, message)
			{
			}
		}

		public class Handler : IRequestHandler<Command, AssignChatSessionResult>
		{
			private readonly IChatSessionRepository _chatSessionRepository;
			private readonly IQueueRepository _queueRepository;
			private readonly IAssignmentIndexLogRepository _assignmentIndexLogRepository;

			public Handler(
					IChatSessionRepository chatSessionRepository,
					IQueueRepository queueRepository,
					IAssignmentIndexLogRepository assignmentIndexLogRepository)
			{
				_chatSessionRepository = chatSessionRepository ?? throw new ArgumentNullException(nameof(chatSessionRepository));
				_queueRepository = queueRepository ?? throw new ArgumentNullException(nameof(queueRepository));
				_assignmentIndexLogRepository = assignmentIndexLogRepository ?? throw new ArgumentNullException(nameof(assignmentIndexLogRepository));
			}

			public async Task<AssignChatSessionResult> Handle(Command request, CancellationToken cancellationToken)
			{
				var chatSession = await _chatSessionRepository.GetByIdAsync(request.SessionId);
				var queueInfo = await _queueRepository.GetByIdAsync(chatSession.QueueId.Value);

				chatSession.AgentId = request.AgentId;
				chatSession.Status = ChatStatus.Active;

				var updateSuccess = await _chatSessionRepository.AssignAgentToSessionAsync(chatSession.Id, request.AgentId);
				if (!updateSuccess)
				{
					return new AssignChatSessionResult(HttpStatusCode.InternalServerError, "Failed to assign agent to chat session");
				}

				await UpdateAssignmentIndexLogAsync(request, queueInfo.QueueName);

				return new AssignChatSessionResult("Chat session successfully assigned to agent");
			}

			private async Task UpdateAssignmentIndexLogAsync(Command request, string queueName)
			{
				var latestIndexLog = await _assignmentIndexLogRepository.GetLatestIndexLogAsync();

				if (latestIndexLog != null)
				{
					latestIndexLog.CurrentAgentIndex++;
					await _assignmentIndexLogRepository.UpdateAsync(latestIndexLog);
				}
				else
				{
					var newAssignmentIndexLog = new AssignmentIndexLogVersion
					{
						Id = Guid.NewGuid(),
						AgentId = request.AgentId,
						ChatSessionId = request.SessionId,
						ChatStatus = ChatStatus.Active,
						QueueName = queueName,
						CurrentAgentIndex = 1
					};

					await _assignmentIndexLogRepository.AddAsync(newAssignmentIndexLog);
				}
			}
		}
	}
}

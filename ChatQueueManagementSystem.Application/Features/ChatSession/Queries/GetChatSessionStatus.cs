using ChatQueueManagementSystem.Application.Common.Exceptions;
using ChatQueueManagementSystem.Application.Common.Helpers.Mapper;
using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Domain.Enums;
using MediatR;

namespace ChatQueueManagementSystem.Application.Features.ChatSession.Queries
{
	public static class GetChatSessionStatus
	{
		public class Query : IRequest<ChatSessionStatusResult>
		{
			public Guid SessionId { get; set; }
		}

		public class ChatSessionStatusResult
		{
			public Guid SessionId { get; set; }
			public Guid? AgentId { get; set; }
			public Guid? UserId { get; set; }
			public Guid? QueueId { get; set; }
			public ChatStatus Status { get; set; }
			public string Message { get; set; }
			public bool IsActive { get; set; }
			public DateTime StartTime { get; set; }
			public DateTime? EndTime { get; set; }
		}

		public class Handler : IRequestHandler<Query, ChatSessionStatusResult>
		{
			private readonly IChatSessionRepository _chatSessionRepository;

			public Handler(IChatSessionRepository chatSessionRepository)
			{
				_chatSessionRepository = chatSessionRepository;
			}

			public async Task<ChatSessionStatusResult> Handle(Query request, CancellationToken cancellationToken)
			{
				var chatSession = await _chatSessionRepository.GetByIdAsync(request.SessionId);
			
				if (chatSession == null)
				{
					throw new NotFoundException(nameof(ChatSession), request.SessionId);

				}

				return chatSession.GetResponse();
			}
		}
	}
}

using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Application.Services;
using ChatQueueManagementSystem.Domain.Enums;
using MediatR;
using System.Net;

namespace ChatQueueManagementSystem.Application.Features.ChatSession.Commands
{
	public static class CreateChatSession
	{
		public class Command : IRequest<Result>
		{
			public Guid UserId { get; set; }
			public string Message { get; set; }
			public DateTime StartTime { get; set; }
		}

		public class Result
		{
			public Guid ChatSessionId { get; set; }
			public HttpStatusCode StatusCode { get; set; }
			public string Message { get; set; }

			public Result(string message, HttpStatusCode statusCode)
			{
					Message = message;
					StatusCode = statusCode;
			}

			public Result(Guid chatSessionId, string message)
			{
				StatusCode = HttpStatusCode.OK;
				ChatSessionId = chatSessionId;
				Message = message;
			}
		}

		public class Handler: IRequestHandler<Command, Result>
		{
			private readonly IUserRepository _userRepository;
			private readonly IChatService _chatService;
			private readonly IQueueRepository _queueRepository;

			public Handler(IUserRepository userRepository, IChatService chatService, IQueueRepository queueRepository)
			{
				_userRepository = userRepository;
				_chatService = chatService;
				_queueRepository = queueRepository;
			}

			public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
			{
				var user = await _userRepository.GetByIdAsync(request.UserId);

				if (user == null)
				{
					return new Result("User not found", HttpStatusCode.NotFound);
				}

				var queueInfo = await _queueRepository.GetQueueByTypeAsync(false);

				var chatSession = new Domain.Entities.ChatSession
				{
					Id = Guid.NewGuid(),
					UserId = user.Id,
					QueueId = queueInfo.Id,
					StartTime = request.StartTime,
					Status = ChatStatus.Queued,
					IsActive = false,
					Message = request.Message
				};

				await _chatService.CreateChatSessionAsync(chatSession, queueInfo.QueueName);

				return new Result(chatSession.Id, nameof(HttpStatusCode.Created));
			}
		}
	}
}
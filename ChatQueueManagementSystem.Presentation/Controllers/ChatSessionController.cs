using ChatQueueManagementSystem.Application.Features.ChatSession.Commands;
using ChatQueueManagementSystem.Application.Features.ChatSession.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatQueueManagementSystem.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ChatSessionController : BaseController
	{
		public ChatSessionController(IMediator mediator, ILogger<ChatSessionController> logger) : base(mediator, logger)
		{
		}

		[HttpPost("chat-session")]
		public async Task<IActionResult> CreateChatSessionAsync([FromBody] CreateChatSession.Command command)
		{
			try
			{
				var result = await Mediator.Send(command);
				return HandleResult(result);
			}
			catch (Exception ex)
			{
				return HandleError(ex);
			}
		}

		[HttpGet("chat-session/{sessionId}/status")]
		public async Task<IActionResult> GetChatSessionAsync([FromRoute] Guid sessionId)
		{
			try
			{
				var result = await Mediator.Send(new GetChatSessionStatus.Query { SessionId = sessionId });
				return HandleResult(result);
			}
			catch (Exception ex)
			{
				return HandleError(ex);
			}
		}

		[HttpPost("chat-sessions/{sessionId}/assign/{agentId}/agent")]
		public async Task<IActionResult> AssignChatSessionAsync([FromRoute] Guid sessionId, [FromRoute] Guid agentId)
		{
			try
			{
				var result = await Mediator.Send(new AssignChatSessionToAvailableAgent.Command { SessionId = sessionId, AgentId = agentId });
				return HandleResult(result);
			}
			catch (Exception ex)
			{
				return HandleError(ex);
			}
		}
	}
}

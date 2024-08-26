using ChatQueueManagementSystem.Application.Features.Team.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatQueueManagementSystem.Presentation.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TeamController : BaseController
	{
		public TeamController(IMediator mediator, ILogger<TeamController> logger) : base(mediator, logger)
		{
		}

		[HttpGet("team-capacity")]
		[ProducesResponseType(typeof(GetAllTeamsCapacity.TeamsCapacityResult), 200)]
		public async Task<IActionResult> GetTeamCapacity([FromQuery] GetAllTeamsCapacity.Query query)
		{
			try
			{
				var result = await Mediator.Send(query);
				return HandleResult(result);
			}
			catch (Exception ex)
			{
				return HandleError(ex);
			}
		}
	}
}
using ChatQueueManagementSystem.Application.Common.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatQueueManagementSystem.Presentation.Controllers
{
	public abstract class BaseController : ControllerBase
	{
		protected readonly IMediator Mediator;
		private readonly ILogger<BaseController> _logger;

		protected BaseController(IMediator mediator, ILogger<BaseController> logger)
		{
			Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		protected IActionResult HandleResult<T>(T result)
		{
			if (result == null)
			{
				LoggingHelper.LogInformation(_logger, HttpContext.Request, JsonConvert.SerializeObject(result));
				return NotFound();
			}

			LoggingHelper.LogInformation(_logger, HttpContext.Request, JsonConvert.SerializeObject(result));
			return Ok(result);
		}

		protected IActionResult HandleError(Exception ex)
		{
			LoggingHelper.LogError(_logger, HttpContext.Request, ex.Message);
			return StatusCode(500, new { Error = ex.Message });
		}
	}
}
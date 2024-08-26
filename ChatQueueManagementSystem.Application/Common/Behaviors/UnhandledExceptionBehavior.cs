using MediatR;
using Microsoft.Extensions.Logging;

namespace ChatQueueManagementSystem.Application.Common.Behaviors
{
	public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
	{
		private readonly ILogger<TRequest> _logger;

		public UnhandledExceptionBehavior(ILogger<TRequest> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			try
			{
				return await next();
			}
			catch (Exception ex)
			{
				var requestName = typeof(TRequest).Name;

				_logger.LogError(ex, "Application Request: unhandled exception for {Name} {@Request}", requestName, request);
				throw;
			}
		}
	}
}
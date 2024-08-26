using FluentValidation;
using MediatR;
using ValidationException = ChatQueueManagementSystem.Application.Common.Exceptions.ValidationException;

namespace ChatQueueManagementSystem.Application.Common.Behaviors
{
	public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
	{
		private readonly IEnumerable<IValidator<TRequest>> _validator;

		public ValidationBehavior(IEnumerable<IValidator<TRequest>> validator)
		{
			_validator = validator ?? throw new ArgumentNullException(nameof(validator));
		}


		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			if (!_validator.Any()) return await next();
			var context = new ValidationContext<TRequest>(request);

			var validationResult = await Task.WhenAll(_validator.Select(x => x.ValidateAsync(context, cancellationToken)));

			var failures = validationResult.SelectMany(x => x.Errors).Where(x => x != null).ToList();

			if (failures.Count != 0)
			{
				throw new ValidationException(failures);
			}

			return await next();
		}
	}
}
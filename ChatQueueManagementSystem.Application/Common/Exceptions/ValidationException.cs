using FluentValidation.Results;

namespace ChatQueueManagementSystem.Application.Common.Exceptions
{
	class ValidationException : ApplicationException
	{
		public ValidationException() : base("one or more errors have occured")
		{
			Errors = new Dictionary<string, string[]>();
		}

		public ValidationException(IEnumerable<ValidationFailure> failures) : this()
		{
			Errors = failures
				.GroupBy(error => error.PropertyName, error => error.ErrorMessage)
				.ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
		}

		public Dictionary<string, string[]> Errors { get; }

		public override string Message => string.Join("; ", Errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}"));
	}
}
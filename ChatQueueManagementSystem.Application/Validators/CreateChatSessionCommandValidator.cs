using ChatQueueManagementSystem.Application.Features.ChatSession.Commands;
using FluentValidation;

namespace ChatQueueManagementSystem.Application.Validators
{
	public class CreateChatSessionCommandValidator : AbstractValidator<CreateChatSession.Command>
	{
		public CreateChatSessionCommandValidator()
		{
			RuleFor(p => p.Message)
					.NotEmpty().WithMessage("{Message} is required.")
					.NotNull();
		}
	}
}
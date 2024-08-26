using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Application.Features.ChatSession.Commands;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Domain.Enums;
using FluentValidation;

namespace ChatQueueManagementSystem.Application.Validators
{
	public class AssignChatSessionCommandValidator : AbstractValidator<AssignChatSessionToAvailableAgent.Command>
	{
		public AssignChatSessionCommandValidator(
				IChatSessionRepository chatSessionRepository,
				IAgentRepository agentRepository,
				IQueueRepository queueRepository)
		{
			ChatSession? chatSession = null;
			Agent? agent = null;

			RuleFor(x => x.SessionId)
					.NotEmpty().WithMessage("SessionId must not be empty.")
					.MustAsync(async (sessionId, cancellationToken) =>
					{
						chatSession = await chatSessionRepository.GetByIdAsync(sessionId);
						return chatSession != null;
					}).WithMessage("Chat session not found.");

			RuleFor(x => x.AgentId)
					.NotEmpty().WithMessage("AgentId must not be empty.")
					.MustAsync(async (agentId, cancellationToken) =>
					{
						agent = await agentRepository.GetByIdAsync(agentId);
						return agent != null;
					}).WithMessage("Agent not found.");

			RuleFor(x => x)
					.Must(x => chatSession != null && chatSession.Status == ChatStatus.Queued)
					.WithMessage("Chat session is not in a queued state.")
					.When(x => chatSession != null);

			RuleFor(x => x)
					.Must(x => chatSession != null && !chatSession.AgentId.HasValue)
					.WithMessage("Chat session is already assigned to an agent.")
					.When(x => chatSession != null);

			RuleFor(x => x)
					.Must(x => chatSession != null && chatSession.QueueId.HasValue)
					.WithMessage("Chat session is not associated with a queue.")
					.When(x => chatSession != null);

			RuleFor(x => x)
					.MustAsync(async (command, cancellationToken) =>
					{
						if (chatSession == null || !chatSession.QueueId.HasValue || agent == null) return false;

						var queueInfo = await queueRepository.GetByIdAsync(chatSession.QueueId.Value);
						if (queueInfo == null) return false;

						var assignedChatsCount = queueInfo.ChatSessions.Count(x => x.Status == ChatStatus.Queued && x.AgentId == command.AgentId);
						return assignedChatsCount < agent.MaxConcurrentChats && agent.CurrentConcurrentChats < agent.MaxConcurrentChats;
					}).WithMessage("Agent is at full capacity.")
					.When(x => chatSession != null && agent != null);
		}
	}
}

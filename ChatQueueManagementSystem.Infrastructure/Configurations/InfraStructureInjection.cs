using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Application.Services;
using ChatQueueManagementSystem.Infrastructure.Extensions;
using ChatQueueManagementSystem.Infrastructure.Messaging;
using ChatQueueManagementSystem.Infrastructure.Repositories;
using ChatQueueManagementSystem.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatQueueManagementSystem.Infrastructure.Configurations
{
	public static class InfraStructureInjection
	{
		public static async Task<IServiceCollection> AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
			services.AddScoped<IQueueRepository, QueueRepository>();
			services.AddScoped<IAgentRepository, AgentRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<ITeamRepository, TeamRepository>();
			services.AddScoped<IAssignmentIndexLogRepository, AssignmentIndexLogRepository>();
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			services.AddScoped<IChatService, ChatService>();
			services.AddScoped<IChatQueueService, ChatQueueService>();

			await services.AddRabbitMqServices(configuration);
			services.AddHostedService<MonitorChatSessionsTask>();

			return services;
		}
	}
}
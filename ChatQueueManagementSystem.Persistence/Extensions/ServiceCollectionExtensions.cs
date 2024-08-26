using ChatQueueManagementSystem.Persistence.Context;
using ChatQueueManagementSystem.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatQueueManagementSystem.Persistence.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ChatQueueDbContext>(options =>
				options.UseSqlServer(configuration.GetConnectionString("ChatQueueDBConnection"),
					getAssembly => getAssembly.MigrationsAssembly(typeof(ChatQueueDbContext).Assembly.FullName)));

			services.AddScoped<ChatQueueDbContextSeeder>();

			return services;
		}
	}
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ChatQueueManagementSystem.Persistence.Context
{
	public class ChatQueueDbContextFactory : IDesignTimeDbContextFactory<ChatQueueDbContext>
	{
		public ChatQueueDbContext CreateDbContext(string[] args)
		{
			// Build configuration
			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();

			// Set up DbContext options
			var optionsBuilder = new DbContextOptionsBuilder<ChatQueueDbContext>();
			var connectionString = configuration.GetConnectionString("ChatQueueDBConnection");

			optionsBuilder.UseSqlServer(connectionString,
				getAssembly => getAssembly.MigrationsAssembly(typeof(ChatQueueDbContext).Assembly.FullName));

			return new ChatQueueDbContext(optionsBuilder.Options);
		}
	}
}
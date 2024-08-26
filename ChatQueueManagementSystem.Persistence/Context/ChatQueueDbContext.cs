using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace ChatQueueManagementSystem.Persistence.Context
{
    public class ChatQueueDbContext : DbContext
	{
		public ChatQueueDbContext(DbContextOptions<ChatQueueDbContext> options) : base(options) { }

		public DbSet<Agent> Agents { get; set; }
		public DbSet<ChatSession> ChatSessions { get; set; }
		public DbSet<Overflow> Overflows { get; set; }
		public DbSet<Queue> Queues { get; set; }
		public DbSet<Team> Teams { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<AssignmentIndexLogVersion> AssignmentIndexLogVersions { get; set; }

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			foreach (var entry in ChangeTracker.Entries<BaseEntity>())
			{
				switch (entry.State)
				{
					case EntityState.Added:
						entry.Entity.CreatedAt = DateTime.UtcNow;
						break;
					case EntityState.Modified:
						entry.Entity.UpdatedAt = DateTime.UtcNow;
						break;
					case EntityState.Deleted:
						entry.Entity.IsDeleted = true;
						break;
				}
			}


			return base.SaveChangesAsync(cancellationToken);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Agent>()
				.HasIndex(a => a.Name)
				.HasDatabaseName("IX_Agent_Name");

			modelBuilder.Entity<Agent>()
				.HasIndex(a => a.TeamId)
				.HasDatabaseName("IX_Agent_TeamId");

			modelBuilder.Entity<ChatSession>()
				.HasIndex(cs => cs.UserId)
				.HasDatabaseName("IX_ChatSession_UserId");

			modelBuilder.Entity<ChatSession>()
				.HasIndex(cs => cs.AgentId)
				.HasDatabaseName("IX_ChatSession_AgentId");

			modelBuilder.Entity<ChatSession>()
				.HasIndex(cs => cs.Status)
				.HasDatabaseName("IX_ChatSession_Status");

			modelBuilder.Entity<Queue>()
				.HasIndex(q => q.QueueName)
				.HasDatabaseName("IX_Queue_QueueName");

			modelBuilder.Entity<Team>()
				.HasIndex(t => t.Name)
				.HasDatabaseName("IX_Team_Name");

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.HasDatabaseName("IX_User_Email");

			modelBuilder.Entity<AssignmentIndexLogVersion>()
				.HasIndex(a => new { a.AgentId, a.ChatSessionId, a.CurrentAgentIndex })
				.HasDatabaseName("IX_AssignmentIndexLogVersion_AgentId_ChatSessionId_CurrentAgentIndex");
		}
	}
}

using ChatQueueManagementSystem.Application.Common.Constants;
using ChatQueueManagementSystem.Application.Common.Helpers;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Domain.Enums;
using ChatQueueManagementSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatQueueManagementSystem.Persistence.Seeding
{
	public class ChatQueueDbContextSeeder
	{
		private readonly ChatQueueDbContext _context;
		private readonly ILogger<ChatQueueDbContextSeeder> _logger;

		public ChatQueueDbContextSeeder(ChatQueueDbContext context, ILogger<ChatQueueDbContextSeeder> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task SeedAsync()
		{
			try
			{
				if (!_context.Teams.Any())
				{
					await SeedTeamsAsync();
				}

				if (!_context.Users.Any())
				{
					await SeedUsersAsync();
				}

				if (!_context.Agents.Any())
				{
					await SeedAgents();
				}

				if (!_context.Queues.Any())
				{
					await SeedQueues();
				}

				if (!_context.ChatSessions.Any())
				{
					await SeedChatSessions();
				}

				if (!_context.Overflows.Any())
				{
					await SeedOverflowTeams();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while seeding the database.");
				throw;
			}
		}

		private async Task SeedTeamsAsync()
		{
			var teams = new List<Team>
			{
				new Team
				{
					Id = Guid.NewGuid(),
					Name = TeamGroup.TeamA,
					Agents = new List<Agent>
					{
						new Agent
						{
							Id = Guid.NewGuid(),
							Name = "Agent 1",
							SeniorityLevel = SeniorityLevel.TeamLead,
							SeniorityMultiplier = 0.5,
							MaxConcurrentChats = 0,
							CurrentConcurrentChats = 0,
							ShiftStartTime = TimeSpan.FromHours(8),
							ShiftDuration = TimeSpan.FromHours(8)
						},
						new Agent
						{
							Id = Guid.NewGuid(),
							Name = "Agent 2",
							SeniorityLevel = SeniorityLevel.MidLevel,
							SeniorityMultiplier = 0.6,
							MaxConcurrentChats = 10,
							CurrentConcurrentChats = 0,
							ShiftStartTime = TimeSpan.FromHours(8),
							ShiftDuration = TimeSpan.FromHours(8)
						},
						new Agent
						{
							Id = Guid.NewGuid(),
							Name = "Agent 3",
							SeniorityLevel = SeniorityLevel.MidLevel,
							SeniorityMultiplier = 0.6,
							MaxConcurrentChats = 10,
							CurrentConcurrentChats = 0,
							ShiftStartTime = TimeSpan.FromHours(8),
							ShiftDuration = TimeSpan.FromHours(8)
						},
						new Agent
						{
							Id = Guid.NewGuid(),
							Name = "Agent 4",
							SeniorityLevel = SeniorityLevel.Junior,
							SeniorityMultiplier = 0.4,
							MaxConcurrentChats = 10,
							CurrentConcurrentChats = 0,
							ShiftStartTime = TimeSpan.FromHours(8),
							ShiftDuration = TimeSpan.FromHours(8)
						}
					}
				},
				new Team
				{
					Id = Guid.NewGuid(),
					Name = TeamGroup.OverFlowTeam,
					Agents = new List<Agent>
					{
						new Agent
						{
							Id = Guid.NewGuid(),
							Name = "Agent 1",
							SeniorityLevel = SeniorityLevel.Senior,
							SeniorityMultiplier = 0.8,
							MaxConcurrentChats = 10,
							CurrentConcurrentChats = 0,
							ShiftStartTime = TimeSpan.FromHours(8),
							ShiftDuration = TimeSpan.FromHours(8),
							CreatedAt = DateTime.UtcNow
						},
						new Agent
						{
							Id = Guid.NewGuid(),
							Name = "Agent 2",
							SeniorityLevel = SeniorityLevel.MidLevel,
							SeniorityMultiplier = 0.6,
							MaxConcurrentChats = 10,
							CurrentConcurrentChats = 0,
							ShiftStartTime = TimeSpan.FromHours(8),
							ShiftDuration = TimeSpan.FromHours(8),
							CreatedAt = DateTime.UtcNow
						},
						new Agent
						{
							Id = Guid.NewGuid(),
							Name = "Agent 3",
							SeniorityLevel = SeniorityLevel.Junior,
							SeniorityMultiplier = 0.4,
							MaxConcurrentChats = 10,
							CurrentConcurrentChats = 0,
							ShiftStartTime = TimeSpan.FromHours(8),
							ShiftDuration = TimeSpan.FromHours(8),
							CreatedAt = DateTime.UtcNow
						},
						new Agent
						{
							Id = Guid.NewGuid(),
							Name = "Agent 4",
							SeniorityLevel = SeniorityLevel.Junior,
							SeniorityMultiplier = 0.4,
							CurrentConcurrentChats = 0,
							ShiftStartTime = TimeSpan.FromHours(8),
							ShiftDuration = TimeSpan.FromHours(8),
							CreatedAt = DateTime.UtcNow
						}
					}
				}
			};

			var agents = teams.SelectMany(t => t.Agents).ToList();
			foreach (var team in agents)
			{
				 team.MaxConcurrentChats = ChatsHelper.CalculateCurrentChatCapacity(new List<Agent> { team });
			}
			_context.Teams.AddRange(teams);
			await _context.SaveChangesAsync();
		}

		private async Task SeedUsersAsync()
		{
			var users = new List<User>
			{
				new User { Id = Guid.NewGuid(), Name = "User 1", Email = "user1@example.com", PhoneNumber = "1234567890", CreatedAt = DateTime.UtcNow},
				new User { Id = Guid.NewGuid(), Name = "User 2", Email = "user2@example.com", PhoneNumber = "0987654321", CreatedAt = DateTime.UtcNow },
			};

			_context.Users.AddRange(users);
			await _context.SaveChangesAsync();
		}

		private async Task SeedAgents()
		{
			var teamA = await _context.Teams.FirstAsync(t => t.Name == TeamGroup.TeamA);
			var teamB = await _context.Teams.FirstAsync(t => t.Name == TeamGroup.TeamB);

			var agents = new List<Agent>
			{
				new Agent
				{
					Id = Guid.NewGuid(),
					TeamId = teamA.Id,
					Name = "Agent 1",
					SeniorityLevel = SeniorityLevel.TeamLead,
					MaxConcurrentChats = 0,
					CurrentConcurrentChats = 0,
					ShiftStartTime = TimeSpan.FromHours(8),
					ShiftDuration = TimeSpan.FromHours(8),
					CreatedAt = DateTime.UtcNow
				},
				new Agent
				{
					Id = Guid.NewGuid(),
					Name = "Agent 2",
					TeamId = teamB.Id,
					SeniorityLevel = SeniorityLevel.MidLevel,
					MaxConcurrentChats = 0,
					CurrentConcurrentChats = 0,
					ShiftStartTime = TimeSpan.FromHours(8),
					ShiftDuration = TimeSpan.FromHours(8),
					CreatedAt = DateTime.UtcNow
				},
				new Agent
				{
					Id = Guid.NewGuid(),
					Name = "Agent 3",
					TeamId = teamB.Id,
					SeniorityLevel = SeniorityLevel.Junior,
					MaxConcurrentChats = 0,
					CurrentConcurrentChats = 0,
					ShiftStartTime = TimeSpan.FromHours(8),
					ShiftDuration = TimeSpan.FromHours(8),
					CreatedAt = DateTime.UtcNow
				}
			};

			foreach (var team in agents)
			{
				team.MaxConcurrentChats = ChatsHelper.CalculateCurrentChatCapacity(new List<Agent> { team });
			}

			teamA.Agents.Add(agents[0]);
			teamA.Agents.Add(agents[1]);
			teamB.Agents.Add(agents[2]);

			await _context.Agents.AddRangeAsync(agents);
			await _context.SaveChangesAsync();
		}

		private async Task SeedQueues()
		{
			var queues = new List<Queue>
			{
				new Queue
				{
					Id = Guid.NewGuid(),
					QueueName = "TestQueueOne",
					QueueLength = 24,
					IsOverflow = false,
					ChatSessions = new List<ChatSession>(),
					CreatedAt = DateTime.UtcNow
				},
				new Queue
				{
					Id = Guid.NewGuid(),
					QueueName = "TestQueueTwo",
					QueueLength = 12,
					IsOverflow = true,
					ChatSessions = new List<ChatSession>(),
					CreatedAt = DateTime.UtcNow
				}
			};

			await _context.Queues.AddRangeAsync(queues);
			await _context.SaveChangesAsync();
		}

		private async Task SeedOverflowTeams()
		{
			var overflowTeams = new Overflow
			{
				Id = Guid.NewGuid(),
				Agents = new List<Agent>()
			};

			var teamA = await _context.Teams.FirstAsync(t => t.Name == TeamGroup.TeamA);
			var teamB = await _context.Teams.FirstAsync(t => t.Name == TeamGroup.TeamA);
			var agentOne = await _context.Agents.FirstAsync(t => t.Name == "Agent 3" && t.SeniorityLevel == SeniorityLevel.Junior);
			var agentTwo = await _context.Agents.FirstAsync(t => t.Name == "Agent 2" && t.SeniorityLevel == SeniorityLevel.MidLevel);

			var overflowAgents = new List<Agent>();

			for (int i = 1; i <= 6; i++)
			{
				if (i < 4)
				{
					overflowAgents.Add(new Agent
					{
						Id = Guid.NewGuid(),
						Name = $"Overflow Agent {i}",
						SeniorityLevel = SeniorityLevel.Junior,
						MaxConcurrentChats = agentOne.MaxConcurrentChats,
						CurrentConcurrentChats = 0,
						TeamId = teamA.Id
					});
				}
				else
				{
					overflowAgents.Add(new Agent
					{
						Id = Guid.NewGuid(),
						Name = $"Overflow Agent {i}",
						SeniorityLevel = SeniorityLevel.MidLevel,
						MaxConcurrentChats = agentTwo.MaxConcurrentChats,
						CurrentConcurrentChats = 0,
						TeamId = teamB.Id
					});
				}
			}

			overflowTeams.Agents.AddRange(overflowAgents);

			await _context.Overflows.AddAsync(overflowTeams);
			await _context.SaveChangesAsync();
		}

		private async Task SeedChatSessions()
		{
			var nonOverflowQueue = await _context.Queues.FirstAsync(q => !q.IsOverflow);
			var overflowQueue = await _context.Queues.FirstAsync(q => q.IsOverflow);

			var agent1 = await _context.Agents.FirstAsync(a => a.Name == "Agent 1");
			var agent2 = await _context.Agents.FirstAsync(a => a.Name == "Agent 2");
			var user1 = await _context.Users.FirstAsync();

			var chatSessions = new List<ChatSession>
			{
				new ChatSession
				{
					Id = Guid.NewGuid(),
					UserId = user1.Id,
					QueueId = nonOverflowQueue.Id,
					AgentId = agent1.Id,
					StartTime = DateTime.UtcNow,
					EndTime = DateTime.UtcNow,
					Status = ChatStatus.Refused,
					IsActive = false,
					Message = string.Empty
				},
				new ChatSession
				{
					Id = Guid.NewGuid(),
					UserId = user1.Id,
					QueueId = overflowQueue.Id,
					AgentId = agent2.Id,
					StartTime = DateTime.UtcNow,
					EndTime = DateTime.UtcNow,
					Status = ChatStatus.Queued,
					IsActive = true,
					Message = string.Empty
				}
			};

			await _context.ChatSessions.AddRangeAsync(chatSessions);
			await _context.SaveChangesAsync();
		}
	}
}
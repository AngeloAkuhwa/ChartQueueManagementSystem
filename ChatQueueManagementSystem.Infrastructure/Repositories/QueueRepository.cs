using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatQueueManagementSystem.Infrastructure.Repositories
{
	public class QueueRepository : GenericRepository<Queue>, IQueueRepository
	{
		public QueueRepository(ChatQueueDbContext context) : base(context)
		{
		}

		public async Task<Queue?> GetQueueByTypeAsync(bool isOverflow)
		{
			var queue = await Context.Queues.FirstOrDefaultAsync(q => q.IsOverflow == isOverflow);
			return 	 queue ?? new Queue();
		}

		public async Task<Queue?> GetQueueByNameAsync(string queueName)
		{
			var queue = await Context.Queues.FirstOrDefaultAsync(q => q.QueueName == queueName);
			return queue ?? new Queue();
		}

		public async Task<bool> AddChatSessionToQueueAsync(Guid queueId, ChatSession chatSession)
		{
			var queue = await Context.Queues.FirstOrDefaultAsync(q => q.Id == queueId);

			if (queue == null) return false;

			var queueCount = queue.ChatSessions.Count;
			queue.ChatSessions.Add(chatSession);
			await SaveChangesAsync();

			return queue.ChatSessions.Count > queueCount;
		}

		public async Task<bool> RemoveChatSessionFromQueueAsync(Guid queueId, Guid chatSessionId)
		{
			var queue = await Context.Queues.FirstOrDefaultAsync(q => q.Id == queueId);

			if(queue == null || !queue.ChatSessions.Any()) return false;

			var session = queue.ChatSessions.FirstOrDefault(cs => cs.Id == chatSessionId);

			if(session == null) return false;

			var chatSessionsCount = queue.ChatSessions.Count;
			queue.ChatSessions.Remove(session);
			await SaveChangesAsync();

			return chatSessionsCount > queue.ChatSessions.Count;
		}
	}
}
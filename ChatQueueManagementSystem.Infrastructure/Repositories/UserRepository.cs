using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using ChatQueueManagementSystem.Domain.Entities;
using ChatQueueManagementSystem.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ChatQueueManagementSystem.Infrastructure.Repositories
{
	public class UserRepository : GenericRepository<User>, IUserRepository
	{
		public UserRepository(ChatQueueDbContext context) : base(context)
		{
		}

		public async Task<User?> GetUserByEmailAsync(string email)
		{
			return await Context.Users.FirstOrDefaultAsync(u => u.Email == email);
		}

		public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
		{
			return await Context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
		}
	}
}
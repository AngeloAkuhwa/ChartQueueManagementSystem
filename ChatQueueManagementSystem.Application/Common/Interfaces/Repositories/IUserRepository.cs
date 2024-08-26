using ChatQueueManagementSystem.Domain.Entities;

namespace ChatQueueManagementSystem.Application.Common.Interfaces.Repositories
{
	public interface IUserRepository : IGenericRepository<User>
	{
		Task<User?> GetUserByEmailAsync(string email);
		Task<User?> GetUserByPhoneNumberAsync(string phoneNumber);
	}
}

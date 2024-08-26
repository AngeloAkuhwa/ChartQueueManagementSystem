namespace ChatQueueManagementSystem.Application.Common.Interfaces.Repositories
{
	public interface IGenericRepository<TEntity> where TEntity : class
	{
		Task<TEntity?> GetByIdAsync(Guid id);
		Task<IEnumerable<TEntity>> GetAllAsync();
		IQueryable<TEntity> GetAllAsQueryable();
		Task AddAsync(TEntity entity);
		Task UpdateAsync(TEntity entity);
		Task DeleteAsync(TEntity entity);
		Task SaveChangesAsync();
	}
}
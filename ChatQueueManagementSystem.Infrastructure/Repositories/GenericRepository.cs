using ChatQueueManagementSystem.Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using ChatQueueManagementSystem.Persistence.Context;
using ChatQueueManagementSystem.Domain.Entities;

namespace ChatQueueManagementSystem.Infrastructure.Repositories
{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
	{
		protected readonly ChatQueueDbContext Context;
		private readonly DbSet<TEntity> _dbSet;

		public GenericRepository(ChatQueueDbContext context)
		{
			Context = context;
			_dbSet = Context.Set<TEntity>();
		}

		public async Task<TEntity?> GetByIdAsync(Guid id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task<IEnumerable<TEntity>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public IQueryable<TEntity> GetAllAsQueryable()
		{
			return _dbSet.AsQueryable();
		}

		public async Task AddAsync(TEntity entity)
		{
			await _dbSet.AddAsync(entity);
			await SaveChangesAsync();
		}

		public async Task UpdateAsync(TEntity entity)
		{
			await Task.Run(() => _dbSet.Update(entity));
			await SaveChangesAsync();
		}

		public async Task DeleteAsync(TEntity entity)
		{
			await Task.Run(() => _dbSet.Remove(entity));
			await SaveChangesAsync();
		}


		public async Task SaveChangesAsync()
		{
			await Context.SaveChangesAsync();
		}
	}
}
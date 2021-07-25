using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProjectHubDTO;

namespace Infrastructure.Data
{
    public class EfRepository<T> : IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationContext _dbContext;

        public EfRepository(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var keys = new object[] { id };

            return await _dbContext.Set<T>().FindAsync(keys, cancellationToken);
        }


        public async Task<ICollection<T>> GetAllListAsync(bool isTracking = true, CancellationToken cancellationToken = default)
        {
            var set = _dbContext.Set<T>();

            if (!isTracking) set.AsNoTracking();

            return await set.ToListAsync(cancellationToken);
        }

        public async Task<ICollection<T>> WhereToListAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().Where(expression).ToListAsync(cancellationToken);


        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);

            await SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().Update(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().Remove(entity);

            await SaveChangesAsync(cancellationToken);
        }


        public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().CountAsync(cancellationToken);

        public async Task<int> CountAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().CountAsync(expression, cancellationToken);


        public async Task<T> FirstAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().FirstAsync(cancellationToken);

        public async Task<T> FirstAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().FirstAsync(expression, cancellationToken);


        public async Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().FirstOrDefaultAsync(cancellationToken);

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default) =>
            await _dbContext.Set<T>().FirstOrDefaultAsync(expression, cancellationToken);


        protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

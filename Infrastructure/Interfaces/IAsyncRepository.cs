using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ProjectHubDTO;

namespace Infrastructure.Interfaces
{
    public interface IAsyncRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);


        Task<ICollection<T>> GetAllListAsync(bool isTracking = true, CancellationToken cancellationToken = default);

        Task<ICollection<T>> WhereToListAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);


        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task RemoveAsync(T entity, CancellationToken cancellationToken = default);


        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);


        Task<T> FirstAsync(CancellationToken cancellationToken = default);
        Task<T> FirstAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

        Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    }
}

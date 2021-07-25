using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Data;

namespace Infrastructure.Interfaces
{
    public interface IAuthorRepository : IAsyncRepository<Author>
    {
        Task<Author> GetByIdWithProjectsAsync(int id, CancellationToken cancellationToken = default);
    }
}

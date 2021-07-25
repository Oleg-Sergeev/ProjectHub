using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Data;

namespace Infrastructure.Interfaces
{
    public interface IProjectRepository : IAsyncRepository<Project>
    {
        Task<Project> GetByIdWithAuthorsAsync(int id, CancellationToken cancellationToken = default);

        IQueryable<Project> WithAuthors();
    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ProjectRepository : EfRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<Project> GetByIdWithAuthorsAsync(int id, CancellationToken cancellationToken = default) =>
            await _dbContext.Projects
            .Include(p => p.Authors)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public IQueryable<Project> WithAuthors() =>
            _dbContext.Projects.Include(p => p.Authors);
    }
}

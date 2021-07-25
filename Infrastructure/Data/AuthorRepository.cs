using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AuthorRepository : EfRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<Author> GetByIdWithProjectsAsync(int id, CancellationToken cancellationToken = default) =>
            await _dbContext.Authors
            .Include(a => a.Projects)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}

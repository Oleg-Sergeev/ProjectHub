using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Web.Extensions
{
    public static class DbSetAuthorsExtensions
    {
        public static async Task<Author> GetAuthorWithProjectsAsync(this IQueryable<Author> authors, int id) =>
            await authors
                .Include(a => a.Projects)
                .Where(a => a.Id == id)
                .SingleAsync();
    }
}

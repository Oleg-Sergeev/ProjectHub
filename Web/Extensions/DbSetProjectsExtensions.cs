using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Web.Extensions
{
    public static class DbSetProjectsExtensions
    {
        public static async Task<Project> GetProjectWithProjectsAsync(this IQueryable<Project> projects, int id) =>
            await projects
                .Include(p => p.Authors)
                .Where(p => p.Id == id)
                .SingleAsync();
    }
}

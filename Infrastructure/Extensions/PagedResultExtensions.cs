using System;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data.Pagination;
using Microsoft.EntityFrameworkCore;
using ProjectHubDTO;

namespace Infrastructure.Extensions
{
    public static class PagedResultExtensions
    {
        public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query, int currentPage, int pageSize) where T : BaseEntity
        {
            var skipPages = (currentPage - 1) * pageSize;

            var pagedItems = await query
                .Skip(skipPages)
                .Take(pageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            var totalPages = (int)Math.Ceiling((double)count / pageSize);

            var pagedModel = new PagedViewModel(currentPage, totalPages);

            var result = new PagedResult<T>(pagedItems, pagedModel);


            return result;
        }
    }
}

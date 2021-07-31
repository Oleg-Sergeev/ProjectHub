using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectHubDTO;
using Web.ViewModels.Pagination;

namespace Web.Extensions
{
    public static class PaginationExtensions
    {
        public static async Task<IEnumerable<T>> GetPagedAsync<T>(this IQueryable<T> query, int currentPage, int pageSize) where T : BaseEntity
        {
            var skipPages = (currentPage - 1) * pageSize;

            var pagedItems = await query
                .Skip(skipPages)
                .Take(pageSize)
                .ToListAsync();

            return pagedItems;
        }
    }
}

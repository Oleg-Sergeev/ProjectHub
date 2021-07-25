using System.Collections;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectHubDTO;

namespace Web.Extensions
{
    public static class AsyncRepositoryExtensions
    {
        public static async Task<MultiSelectList> CreateMultiSelectListAsync<T>(this IAsyncRepository<T> repository, IEnumerable selectedValues = default)
            where T : BaseEntity
        {
            var items = await repository.GetAllListAsync(false);

            return new MultiSelectList(items, selectedValues);
        }
        public static async Task<MultiSelectList> CreateMultiSelectListAsync<T>(this IAsyncRepository<T> repository, string value, string display, IEnumerable selectedValues = default)
            where T : BaseEntity
        {
            var items = await repository.GetAllListAsync(false);

            return new MultiSelectList(items, value, display, selectedValues);
        }
    }
}

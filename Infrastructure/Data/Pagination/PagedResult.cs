using System.Collections.Generic;
using ProjectHubDTO;

namespace Infrastructure.Data.Pagination
{
    public class PagedResult<T> where T : BaseEntity
    {
        public IEnumerable<T> Items { get; }

        public PagedViewModel PageViewModel { get; }


        public PagedResult(IEnumerable<T> items, PagedViewModel pageViewModel)
        {
            Items = items;
            PageViewModel = pageViewModel;
        }
    }
}

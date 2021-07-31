using System.Collections.Generic;
using ProjectHubDTO;

namespace Web.ViewModels.Pagination
{
    public class BasePaginationViewModel<T> where T : BaseEntity
    {
        public IEnumerable<T> Items { get; set; }

        public PaginationInfoViewModel PaginationInfo { get; set; }
    }
}

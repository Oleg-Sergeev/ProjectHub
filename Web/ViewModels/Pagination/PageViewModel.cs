using System;

namespace Web.ViewModels.Pagination
{
    public class PageViewModel
    {
        public int CurrentPage { get; init; }
        public int TotalPages { get; init; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;


        public PageViewModel(int currentPage, int contentCount, int pageSize = 5)
        {
            CurrentPage = currentPage;

            TotalPages = (int)Math.Ceiling((double)contentCount / pageSize);
        }
    }
}

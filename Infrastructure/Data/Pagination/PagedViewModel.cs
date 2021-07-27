using System;

namespace Infrastructure.Data.Pagination
{
    public class PagedViewModel
    {
        public int CurrentPage { get; init; }

        public int TotalPages { get; init; }


        public PagedViewModel(int currentPage, int totalPages)
        {
            CurrentPage = currentPage;

            TotalPages = totalPages;
        }
    }
}

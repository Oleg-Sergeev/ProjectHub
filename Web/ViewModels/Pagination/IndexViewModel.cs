using System.Collections.Generic;
using Infrastructure.Data;

namespace Web.ViewModels.Pagination
{
    public class IndexViewModel
    {
        public IEnumerable<Project> Projects { get; }

        public PageViewModel PageViewModel { get; }


        public IndexViewModel(IEnumerable<Project> projects, PageViewModel pageViewModel)
        {
            Projects = projects;
            PageViewModel = pageViewModel;
        }
    }
}

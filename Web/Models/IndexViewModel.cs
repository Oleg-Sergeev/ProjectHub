using System.Collections.Generic;
using Web.Data;

namespace Web.Models
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

using System.Collections.Generic;
using ProjectHubDTO;

namespace Web.ViewModels
{
    public class ProjectViewModel : ProjectDTO
    {
        public IEnumerable<int> AuthorsId { get; set; }
    }
}

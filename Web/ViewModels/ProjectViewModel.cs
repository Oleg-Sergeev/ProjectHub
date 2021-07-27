using System.Collections.Generic;
using ProjectHubDTO;

namespace Web.ViewModels
{
    public class ProjectViewModel : ProjectDTO
    {
        public List<int> AuthorsId { get; set; } = new();
    }
}

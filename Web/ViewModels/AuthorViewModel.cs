using System.Collections.Generic;
using ProjectHubDTO;

namespace Web.ViewModels
{
    public class AuthorViewModel : AuthorDTO
    {
        public List<int> ProjectsId { get; set; } = new();
    }
}

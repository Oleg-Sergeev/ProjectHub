using System.Collections.Generic;
using ProjectHubDTO;

namespace Web.ViewModels
{
    public class AuthorViewModel : AuthorDTO
    {
        public IEnumerable<int> ProjectsId { get; set; }
    }
}

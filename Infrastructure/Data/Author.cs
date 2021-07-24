using System.Collections.Generic;
using ProjectHubDTO;

namespace Infrastructure.Data
{
    public class Author : AuthorDTO
    {
        public ICollection<Project> Projects { get; set; }
    }
}

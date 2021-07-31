using System.Collections.Generic;
using ProjectHubDTO;

namespace Infrastructure.Data.Entities
{
    public class Author : AuthorDTO
    {
        public ICollection<Project> Projects { get; set; }
    }
}

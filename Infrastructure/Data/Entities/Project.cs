using System.Collections.Generic;
using ProjectHubDTO;

namespace Infrastructure.Data.Entities
{
    public class Project : ProjectDTO
    {
        public ICollection<Author> Authors { get; set; }
    }
}

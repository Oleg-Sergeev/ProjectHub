using System.Collections.Generic;
using ProjectHubDTO;

namespace Infrastructure.Data
{
    public class Project : ProjectDTO
    {
        public ICollection<Author> Authors { get; set; }
    }
}

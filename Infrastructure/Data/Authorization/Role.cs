using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectHubDTO;

namespace Infrastructure.Data.Entities.Authorization
{
    public class Role : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public List<User> Users { get; set; } = new();
    }
}

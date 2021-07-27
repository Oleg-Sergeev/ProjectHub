using System.ComponentModel.DataAnnotations;
using ProjectHubDTO;

namespace Infrastructure.Data.Authorization
{
    public class User : BaseEntity
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public Role Role { get; set; }
    }
}

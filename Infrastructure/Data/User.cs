using System.ComponentModel.DataAnnotations;
using ProjectHubDTO;

namespace Infrastructure.Data
{
    public class User : BaseEntity
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

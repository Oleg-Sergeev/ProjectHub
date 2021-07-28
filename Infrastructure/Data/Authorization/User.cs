using System.ComponentModel.DataAnnotations;
using ProjectHubDTO;

namespace Infrastructure.Data.Authorization
{
    public class User : BaseEntity
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; }

        [Required]
        public Role Role { get; set; }


        public string SecretKey { get; set; }


        public bool HasConfirmedEmail { get; set; }
    }
}

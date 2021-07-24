using System.ComponentModel.DataAnnotations;

namespace ProjectHubDTO
{
    public class AuthorDTO : BaseEntity
    {
        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string LastName { get; set; }


        public string FullName => $"{FirstName} {LastName}";

        public override string ToString() => FullName;
    }
}

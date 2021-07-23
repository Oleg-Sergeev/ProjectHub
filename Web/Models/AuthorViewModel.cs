using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class AuthorViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string LastName { get; set; }

        public IEnumerable<int> ProjectsId { get; set; }


        public string FullName => $"{FirstName} {LastName}";

        public override string ToString() => FullName;
    }
}

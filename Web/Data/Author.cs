using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Data
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string LastName { get; set; }

        public ICollection<Project> Projects { get; set; }


        public string FullName => $"{FirstName} {LastName}";

        public override string ToString() => FullName;
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Data
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Author> Authors { get; set; }


        public override string ToString() => Name;
    }
}

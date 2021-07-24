using System.ComponentModel.DataAnnotations;

namespace ProjectHubDTO
{
    public class ProjectDTO : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }


        public override string ToString() => Name;
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data;

namespace Web.Extensions
{
    public static class ApplicationContextExtensions
    {
        public static async Task InitializeDbAsync(this ApplicationContext context)
        {
            if (context.Authors.Any() || context.Projects.Any()) return;

            var authors = new List<Author>
            {
                new()
                {
                    FirstName = "Олег",
                    LastName = "Сергеев",
                },
                new()
                {
                    FirstName = "Scott",
                    LastName = "Samons"
                },
                new()
                {
                    FirstName = "Даниил",
                    LastName = "Лапаев"
                }
            };

            var projects = new List<Project>
            {
                new()
                {
                    Name = "Discord bot C#",
                    Description = "some description",
                    Authors = new List<Author>()
                },
                new()
                {
                    Name = "Волк",
                    Description = "Ауф",
                    Authors = new List<Author>()
                },
                new()
                {
                    Name = "Koking Mucker",
                    Authors = new List<Author>()
                }
            };


            await context.Authors.AddRangeAsync(authors);
            await context.Projects.AddRangeAsync(projects);

            authors[0].Projects = new List<Project> { projects[0], projects[2] };
            authors[1].Projects = new List<Project> { projects[2] };
            authors[2].Projects = new List<Project> { projects[1], projects[2] };

            await context.SaveChangesAsync();
        }
    }
}

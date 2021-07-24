using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;

namespace Infrastructure.Extensions
{
    public static class ApplicationContextSeed
    {
        public static async Task SeedAsync(this ApplicationContext context)
        {
            if (context.Authors.Any() || context.Projects.Any()) return;

            var authors = GetAuthors();
            var projects = GetProjects();

            await context.Authors.AddRangeAsync(authors);
            await context.Projects.AddRangeAsync(projects);

            MakeManyToMany(authors, projects);

            await context.SaveChangesAsync();
        }


        private static IList<Author> GetAuthors() =>
            new List<Author>
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

        private static IList<Project> GetProjects() =>
            new List<Project>
            {
                new()
                {
                    Name = "Discord bot C#",
                    Description = "some description"
                },
                new()
                {
                    Name = "Волк",
                    Description = "Ауф"
                },
                new()
                {
                    Name = "Koking Mucker"
                }
            };

        private static void MakeManyToMany(IList<Author> authors, IList<Project> projects)
        {
            authors[0].Projects = new List<Project> { projects[0], projects[2] };
            authors[1].Projects = new List<Project> { projects[2] };
            authors[2].Projects = new List<Project> { projects[1], projects[2] };
        }
    }
}

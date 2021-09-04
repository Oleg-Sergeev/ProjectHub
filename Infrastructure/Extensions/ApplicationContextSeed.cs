using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data.Authorization;
using Infrastructure.Data.Entities;
using Infrastructure.Data.Entities.Authorization;

namespace Infrastructure.Extensions
{
    public static class ApplicationContextSeed
    {
        public static async Task SeedAsync(this ApplicationContext context)
        {
            if (!context.Authors.Any() && !context.Projects.Any())
            {
                var authors = GetAuthors();
                var projects = GetProjects();

                await context.Authors.AddRangeAsync(authors);
                await context.Projects.AddRangeAsync(projects);

                MakeManyToMany(authors, projects);

                await context.SaveChangesAsync();
            }

            if (!context.Roles.Any() && !context.Users.Any())
            {
                var roles = GetRoles();
                var users = GetUsers();

                await context.Roles.AddRangeAsync(roles);
                await context.Users.AddRangeAsync(users);

                AuthorizeUsers(users, roles);

                await context.SaveChangesAsync();
            }
        }


        private static IList<Author> GetAuthors() =>
            new List<Author>
            {
                new()
                {
                    FirstName = "Олег",
                    LastName = "Сергеев",
                }
            };

        private static IList<Project> GetProjects() =>
            new List<Project>
            {
                new()
                {
                    Name = "Project hub",
                    Description = "Simple site to display my projects." +
                    "\n Github link: https://github.com/Oleg-Sergeev/ProjectHub",
                    CreatedAt = new DateTime(2021, 7, 20)
                },
                new()
                {
                    Name = "Discord bot C#",
                    Description = "Created for fun." +
                    "\nGithub link (private repo): https://github.com/Oleg-Sergeev/DiscordBot",
                    CreatedAt = new DateTime(2021, 1, 17)
                },
                new()
                {
                    Name = "The Perfect Way",
                    Description = "My first attempt to create a game." +
                    "\nGoogle play link: https://play.google.com/store/apps/details?id=com.ZixxanGames.PerfectWay " +
                    "\nGithub link: https://github.com/Oleg-Sergeev/ThePerfectWay",
                    CreatedAt = new DateTime(2018, 9, 14)
                },
                new()
                {
                    Name = "The Area 51",
                    Description = "News about \"Area 51\" pushed me to create this game." +
                    "\nGoogle play Link: https://play.google.com/store/apps/details?id=com.ZixxanGames.Area51 " +
                    "\nGithub link: https://github.com/Oleg-Sergeev/TheArea51",
                    CreatedAt = new DateTime(2019, 7, 22)
                },
                new()
                {
                    Name = "Intor",
                    Description = "A project that I will develop for a long time." +
                    "\nGithub link: https://github.com/Oleg-Sergeev/Intor",
                    CreatedAt = new DateTime(2021, 7, 20)
                },
            };

        private static void MakeManyToMany(IList<Author> authors, IList<Project> projects)
        {
            authors[0].Projects = new List<Project>(projects);
        }


        private static IList<Role> GetRoles() =>
            new List<Role>
            {
                new()
                {
                    Name = Constants.AdminRoleName
                },
                new()
                {
                    Name = Constants.UserRoleName
                }
            };

        private static IList<User> GetUsers() =>
            new List<User>
            {
                new()
                {
                    Email = "admin@admin.com",
                    PasswordHash = UserHasher.HashPassword("admin123"),
                    SecretKey = UserHasher.CreateSecretKey(),
                    HasConfirmedEmail = true
                },
                new()
                {
                    Email = "user@user.com",
                    PasswordHash = UserHasher.HashPassword("user123"),
                    SecretKey = UserHasher.CreateSecretKey(),
                    HasConfirmedEmail = true
                }
            };

        private static void AuthorizeUsers(IList<User> users, IList<Role> roles)
        {
            users[0].Role = roles[0];
            users[1].Role = roles[1];
        }
    }
}

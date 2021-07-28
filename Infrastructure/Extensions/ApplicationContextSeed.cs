﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Data.Authorization;
using Microsoft.AspNetCore.Identity;

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
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "admin123"),
                    SecretKey = Convert.ToBase64String(new HMACSHA256().Key),
                    HasConfirmedEmail = true
                },
                new()
                {
                    Email = "user@user.com",
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, "user123"),
                    SecretKey = Convert.ToBase64String(new HMACSHA256().Key),
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

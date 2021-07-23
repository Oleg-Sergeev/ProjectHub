using Microsoft.EntityFrameworkCore;

namespace Web.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<User> Users { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

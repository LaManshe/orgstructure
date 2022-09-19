using Microsoft.EntityFrameworkCore;
using orgstructure.Entities;
using orgstructure.Models.Entities;

namespace orgstructure.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Department> Departments { get; set; } = default!;
        public DbSet<Post> Posts { get; set; } = default!;
    }
}

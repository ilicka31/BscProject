using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Database
{
    public class UserDBContext : DbContext
    {
        public DbSet<Models.User> Users { get; set; }
        public UserDBContext(DbContextOptions<UserDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDBContext).Assembly);
        }
    }
}

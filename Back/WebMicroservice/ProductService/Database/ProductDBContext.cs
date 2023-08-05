using Common.Entites;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Database
{
    public class ProductDBContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<UserInfo> UsersInfo { get; set; }
        public ProductDBContext(DbContextOptions<ProductDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDBContext).Assembly);
        }
    }
}

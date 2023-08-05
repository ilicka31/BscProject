using Common.Entites;
using Common.Entities.Product;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using System.Reflection.Emit;

namespace OrderService.Database
{
    public class OrderDBContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<ProductInfo> ProductInfos { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public OrderDBContext(DbContextOptions<OrderDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDBContext).Assembly);
        }
    }
}

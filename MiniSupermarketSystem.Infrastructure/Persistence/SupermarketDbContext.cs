using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiniSupermarketSystem.Domain.Entities;

namespace MiniSupermarketSystem.Infrastructure.Persistence
{
    // Persistence/SupermarketDbContext.cs
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and constraints
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "Izu",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234567890"),
                    TerminalId = "Izu123",
                    MerchantId = "Izu12",
                },
                new User
                {
                    Id = 2,
                    Username = "Chizoba",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("abcdefgh"),
                    TerminalId = "TM45",
                    MerchantId = "MC12"
                }
            );
        }
    }
}

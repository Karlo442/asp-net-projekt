using HelperZaOptimalnuKupnju.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelperZaOptimalnuKupnju.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public new DbSet<User> Users { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Store> Stores { get; set; } = null!;
        public DbSet<Complaint> Complaints { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Jaja XL", Description = "Svježa kokošja jaja", UnitPrice = 2.50m },
                new Product { Id = 2, Name = "Losos file", Description = "Svježi file lososa 200g", UnitPrice = 15.00m },
                new Product { Id = 3, Name = "Mlijeko 1L", Description = "Pasirano mlijeko 1L", UnitPrice = 1.99m }
            );

            modelBuilder.Entity<Store>().HasData(
                new Store { Id = 1, Name = "Konzum Kaptol", Brand = "Konzum", Address = "Ilica 1", City = "Zagreb", Country = "HR", OpeningHours = "07:00-21:00" },
                new Store { Id = 2, Name = "Lidl Centar", Brand = "Lidl", Address = "Trg 5", City = "Split", Country = "HR", OpeningHours = "07:00-22:00" },
                new Store { Id = 3, Name = "Spar Rijeka", Brand = "Spar", Address = "Korzo 10", City = "Rijeka", Country = "HR", OpeningHours = "08:00-20:00" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "Ivan", LastName = "Horvat", Email = "ivan.horvat@email.com", Role = UserRole.Buyer, Phone = "+38591123456", RegisteredAt = new DateTime(2024, 1, 15), IsActive = true },
                new User { Id = 2, FirstName = "Ana", LastName = "Kovač", Email = "ana.kovac@email.com", Role = UserRole.Buyer, Phone = "+38591765432", RegisteredAt = new DateTime(2024, 2, 20), IsActive = true },
                new User { Id = 3, FirstName = "Marko", LastName = "Babić", Email = "marko.babic@email.com", Role = UserRole.Supplier, Phone = "+38591345678", RegisteredAt = new DateTime(2024, 3, 10), IsActive = true },
                new User { Id = 4, FirstName = "Petra", LastName = "Jurić", Email = "petra.juric@email.com", Role = UserRole.Admin, Phone = "+38591876543", RegisteredAt = new DateTime(2024, 1, 5), IsActive = true }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1, BuyerId = 1, CreatedAt = new DateTime(2026, 3, 10), ExpectedDeliveryDateTime = new DateTime(2026, 3, 15), Status = OrderStatus.Pending, TotalAmount = 15.00m },
                new Order { Id = 2, BuyerId = 2, CreatedAt = new DateTime(2026, 3, 15), ExpectedDeliveryDateTime = new DateTime(2026, 3, 20), Status = OrderStatus.Confirmed, TotalAmount = 69.90m },
                new Order { Id = 3, BuyerId = 1, CreatedAt = new DateTime(2026, 3, 20), ExpectedDeliveryDateTime = new DateTime(2026, 3, 25), Status = OrderStatus.Shipped, TotalAmount = 90.00m }
            );

            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 10, UnitPrice = 1.50m },
                new OrderItem { Id = 2, OrderId = 2, ProductId = 3, Quantity = 10, UnitPrice = 6.99m },
                new OrderItem { Id = 3, OrderId = 3, ProductId = 2, Quantity = 2, UnitPrice = 45.00m }
            );
        }
    }
}

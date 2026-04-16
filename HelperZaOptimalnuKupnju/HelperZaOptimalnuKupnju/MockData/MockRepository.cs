using System;
using System.Collections.Generic;
using System.Linq;
using HelperZaOptimalnuKupnju.Models;

namespace HelperZaOptimalnuKupnju.MockData
{
    public static class MockRepository
    {
        public static IReadOnlyList<User> Users { get; }
        public static IReadOnlyList<Store> Stores { get; }
        public static IReadOnlyList<Product> Products { get; }
        public static IReadOnlyList<Order> Orders { get; }
        public static IReadOnlyList<OrderItem> OrderItems { get; }

        static MockRepository()
        {
            Users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "Ivan",
                    LastName = "Horvat",
                    Email = "ivan.horvat@example.com",
                    Role = UserRole.Buyer,
                    Phone = "+38591234567",
                    RegisteredAt = new DateTime(2024, 1, 15),
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    FirstName = "Ana",
                    LastName = "Ivić",
                    Email = "ana.ivic@example.com",
                    Role = UserRole.Supplier,
                    Phone = "+38591234568",
                    RegisteredAt = new DateTime(2023, 11, 3),
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    FirstName = "Marko",
                    LastName = "Kovač",
                    Email = "marko.kovac@example.com",
                    Role = UserRole.Admin,
                    Phone = "+38591234569",
                    RegisteredAt = new DateTime(2022, 6, 20),
                    IsActive = true
                }
            };

            Products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Jaja XL",
                    Description = "Svježa kokošja jaja",
                    UnitPrice = 1.50m
                },
                new Product
                {
                    Id = 2,
                    Name = "Losos file",
                    Description = "Svježi file lososa 200g",
                    UnitPrice = 45.00m
                },
                new Product
                {
                    Id = 3,
                    Name = "Mlijeko 1L",
                    Description = "Pasirano mlijeko 1L",
                    UnitPrice = 6.99m
                }
            };

            Stores = new List<Store>
            {
                new Store
                {
                    Id = 1,
                    Name = "Konzum Kaptol",
                    Brand = "Konzum",
                    Address = "Ilica 1",
                    City = "Zagreb",
                    Country = "HR",
                    OpeningHours = "07:00-21:00",
                    Products = new List<Product> { Products[0], Products[2] }
                },
                new Store
                {
                    Id = 2,
                    Name = "Lidl Centar",
                    Brand = "Lidl",
                    Address = "Trg 5",
                    City = "Split",
                    Country = "HR",
                    OpeningHours = "07:00-22:00",
                    Products = new List<Product> { Products[1] }
                },
                new Store
                {
                    Id = 3,
                    Name = "Spar Rijeka",
                    Brand = "Spar",
                    Address = "Korzo 10",
                    City = "Rijeka",
                    Country = "HR",
                    OpeningHours = "08:00-20:00",
                    Products = new List<Product>()
                }
            };

            OrderItems = new List<OrderItem>
            {
                new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Product = Products.First(p => p.Id == 1), Quantity = 10, UnitPrice = 1.50m },
                new OrderItem { Id = 2, OrderId = 2, ProductId = 3, Product = Products.First(p => p.Id == 3), Quantity = 10, UnitPrice = 6.99m },
                new OrderItem { Id = 3, OrderId = 3, ProductId = 2, Product = Products.First(p => p.Id == 2), Quantity = 2, UnitPrice = 45.00m }
            };

            Orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    BuyerId = 1,
                    Buyer = Users.First(u => u.Id == 1),
                    CreatedAt = new DateTime(2026, 3, 10),
                    ExpectedDeliveryDateTime = new DateTime(2026, 3, 15),
                    Status = OrderStatus.Pending,
                    TotalAmount = 15.00m,
                    Items = OrderItems.Where(i => i.OrderId == 1).ToList()
                },
                new Order
                {
                    Id = 2,
                    BuyerId = 2,
                    Buyer = Users.First(u => u.Id == 2),
                    CreatedAt = new DateTime(2026, 3, 15),
                    ExpectedDeliveryDateTime = new DateTime(2026, 3, 20),
                    Status = OrderStatus.Confirmed,
                    TotalAmount = 69.90m,
                    Items = OrderItems.Where(i => i.OrderId == 2).ToList()
                },
                new Order
                {
                    Id = 3,
                    BuyerId = 1,
                    Buyer = Users.First(u => u.Id == 1),
                    CreatedAt = new DateTime(2026, 3, 20),
                    ExpectedDeliveryDateTime = new DateTime(2026, 3, 25),
                    Status = OrderStatus.Shipped,
                    TotalAmount = 90.00m,
                    Items = OrderItems.Where(i => i.OrderId == 3).ToList()
                }
            };

            foreach (var item in OrderItems)
            {
                item.Order = Orders.First(o => o.Id == item.OrderId);
            }
        }

        public static User? GetUser(int id) => Users.FirstOrDefault(u => u.Id == id);
        public static Store? GetStore(int id) => Stores.FirstOrDefault(s => s.Id == id);
        public static Product? GetProduct(int id) => Products.FirstOrDefault(p => p.Id == id);
        public static Order? GetOrder(int id) => Orders.FirstOrDefault(o => o.Id == id);
        public static OrderItem? GetOrderItem(int id) => OrderItems.FirstOrDefault(i => i.Id == id);
    }
}

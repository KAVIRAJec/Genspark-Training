using Microsoft.EntityFrameworkCore;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await context.Categories.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed Categories
        var categories = new List<Category>
        {
            new Category { Name = "Smartphones", CreatedDate = DateTime.UtcNow },
            new Category { Name = "Laptops", CreatedDate = DateTime.UtcNow },
            new Category { Name = "Tablets", CreatedDate = DateTime.UtcNow },
            new Category { Name = "Accessories", CreatedDate = DateTime.UtcNow },
            new Category { Name = "Gaming", CreatedDate = DateTime.UtcNow }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // Seed Colors
        var colors = new List<Color>
        {
            new Color { Name = "Black", HexCode = "#000000", CreatedDate = DateTime.UtcNow },
            new Color { Name = "White", HexCode = "#FFFFFF", CreatedDate = DateTime.UtcNow },
            new Color { Name = "Silver", HexCode = "#C0C0C0", CreatedDate = DateTime.UtcNow },
            new Color { Name = "Gold", HexCode = "#FFD700", CreatedDate = DateTime.UtcNow },
            new Color { Name = "Blue", HexCode = "#0000FF", CreatedDate = DateTime.UtcNow },
            new Color { Name = "Red", HexCode = "#FF0000", CreatedDate = DateTime.UtcNow }
        };

        await context.Colors.AddRangeAsync(colors);
        await context.SaveChangesAsync();

        // Seed Models
        var models = new List<Model>
        {
            new Model { Name = "iPhone 15 Pro", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "iPhone 15", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "Galaxy S24 Ultra", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "Galaxy S24", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "MacBook Pro M3", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "MacBook Air M3", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "iPad Pro 6th Gen", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "iPad Air 5th Gen", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "AirPods Pro 2nd Gen", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "PlayStation 5 Slim", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "Xbox Series X", CreatedDate = DateTime.UtcNow, IsActive = true },
            new Model { Name = "Surface Pro 9", CreatedDate = DateTime.UtcNow, IsActive = true }
        };

        await context.Models.AddRangeAsync(models);
        await context.SaveChangesAsync();

        // Seed Users
        var users = new List<User>
        {
            new User
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FirstName = "Admin",
                LastName = "User",
                Phone = "+1234567890",
                Address = "Admin Address",
                Role = "Admin",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            },
            new User
            {
                UserName = "Kaviraj",
                Email = "kaviraj@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Kaviraj@123"),
                FirstName = "Kaviraj",
                LastName = "A",
                Phone = "+1987654321",
                Address = "123 Main St, City, State",
                Role = "User",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // Seed Products
        var products = new List<Product>
        {
            new Product
            {
                ProductName = "iPhone 15 Pro",
                Image = "https://fdn2.gsmarena.com/vv/pics/apple/apple-iphone-15-pro-2.jpg",
                Price = 999.99m,
                CategoryId = categories[0].CategoryId,
                ColorId = colors[0].ColorId,
                ModelId = models[0].ModelId,
                UserId = users[0].UserId,
                SellStartDate = DateTime.UtcNow.AddDays(-30),
                SellEndDate = DateTime.UtcNow.AddDays(365),
                IsNew = 1,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                ProductName = "Samsung Galaxy S24",
                Image = "https://www.shutterstock.com/image-photo/granada-andalusia-spain-24th-january-260nw-2416877835.jpg",
                Price = 899.99m,
                CategoryId = categories[0].CategoryId,
                ColorId = colors[4].ColorId,
                ModelId = models[2].ModelId,
                UserId = users[0].UserId,
                SellStartDate = DateTime.UtcNow.AddDays(-45),
                SellEndDate = DateTime.UtcNow.AddDays(300),
                IsNew = 1,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                ProductName = "MacBook Pro 16\"",
                Image = "https://www.apple.com/v/macbook-pro-14-and-16/a/images/overview/hero/hero_intro_endframe__e6khcva4hkeq_large.jpg",
                Price = 2499.99m,
                CategoryId = categories[1].CategoryId,
                ColorId = colors[2].ColorId,
                ModelId = models[4].ModelId,
                UserId = users[0].UserId,
                SellStartDate = DateTime.UtcNow.AddDays(-60),
                SellEndDate = DateTime.UtcNow.AddDays(400),
                IsNew = 0,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                ProductName = "iPad Pro 12.9\"",
                Image = "https://cdsassets.apple.com/live/SZLF0YNV/images/sp/111977_ipad-pro-12-2020.jpeg",
                Price = 1099.99m,
                CategoryId = categories[2].CategoryId,
                ColorId = colors[1].ColorId,
                ModelId = models[6].ModelId,
                UserId = users[0].UserId,
                SellStartDate = DateTime.UtcNow.AddDays(-20),
                SellEndDate = DateTime.UtcNow.AddDays(500),
                IsNew = 1,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                ProductName = "AirPods Pro",
                Image = "https://m-cdn.phonearena.com/images/hub/274-wide-two_1200/Apple-AirPods-Pro-3-release-date-predictions-price-specs-and-must-know-features.jpg",
                Price = 249.99m,
                CategoryId = categories[3].CategoryId,
                ColorId = colors[1].ColorId,
                ModelId = models[8].ModelId,
                UserId = users[0].UserId,
                SellStartDate = DateTime.UtcNow.AddDays(-10),
                SellEndDate = DateTime.UtcNow.AddDays(200),
                IsNew = 1,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                ProductName = "PlayStation 5",
                Image = "https://t3.ftcdn.net/jpg/05/25/37/30/360_F_525373026_mEvYH9lgyphjWukE2aiqp4O20xSTiZ0g.jpg",
                Price = 499.99m,
                CategoryId = categories[4].CategoryId,
                ColorId = colors[1].ColorId,
                ModelId = models[9].ModelId,
                UserId = users[0].UserId,
                SellStartDate = DateTime.UtcNow.AddDays(-90),
                SellEndDate = DateTime.UtcNow.AddDays(180),
                IsNew = 0,
                CreatedDate = DateTime.UtcNow
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        // Seed News
        var news = new List<News>
        {
            new News
            {
                Title = "New iPhone 15 Pro Available Now!",
                Content = "The latest iPhone 15 Pro is now available in our store with amazing new features including the A17 Pro chip, titanium design, and enhanced camera system.",
                Summary = "iPhone 15 Pro now available with cutting-edge features",
                Image = "https://mobilex.co.in/wp-content/uploads/2023/09/Apple-iPhone-15-Pro-Max-images-2.jpg",
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                IsPublished = true,
                AuthorId = users[0].UserId
            },
            new News
            {
                Title = "Black Friday Sale - Up to 50% Off!",
                Content = "Don't miss our biggest sale of the year! Get up to 50% off on selected smartphones, laptops, and accessories. Sale ends November 30th.",
                Summary = "Massive Black Friday discounts on electronics",
                Image = "https://png.pngtree.com/png-clipart/20200225/original/pngtree-flash-sale-discount-banner-template-promotion-png-image_5305494.jpg",
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                IsPublished = true,
                AuthorId = users[0].UserId
            },
            new News
            {
                Title = "Welcome to Our New Online Store",
                Content = "We're excited to announce the launch of our new and improved online store with better navigation, faster checkout, and mobile-optimized design.",
                Summary = "New online store launched with improved features",
                Image = "https://t4.ftcdn.net/jpg/01/71/56/41/360_F_171564146_hdTBjQUcWNrLe8ev38f8KWi4Qcm7gc4D.jpg",
                CreatedDate = DateTime.UtcNow.AddDays(-15),
                IsPublished = true,
                AuthorId = users[0].UserId
            }
        };

        await context.News.AddRangeAsync(news);
        await context.SaveChangesAsync();

        // Seed Sample Order
        var order = new Order
        {
            UserId = users[1].UserId,
            OrderDate = DateTime.UtcNow.AddDays(-3),
            TotalAmount = 1349.98m,
            Status = "Processing",
            ShippingAddress = "123 Main St, City, State, 12345",
            Notes = "Please call before delivery"
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        // Seed Order Details
        var orderDetails = new List<OrderDetail>
        {
            new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = products[0].ProductId,
                Quantity = 1,
                UnitPrice = 999.99m,
                TotalPrice = 999.99m
            },
            new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = products[4].ProductId,
                Quantity = 1,
                UnitPrice = 249.99m,
                TotalPrice = 249.99m
            }
        };

        await context.OrderDetails.AddRangeAsync(orderDetails);
        await context.SaveChangesAsync();
    }
}

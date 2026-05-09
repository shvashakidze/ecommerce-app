using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Categories
        if (!await db.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Name = "ელექტრონიკა", Slug = "electronics" },
                new() { Name = "ტანსაცმელი", Slug = "clothing" },
                new() { Name = "სპორტი", Slug = "sports" },
                new() { Name = "სახლი და ბაღი", Slug = "home-garden" },
            };
            db.Categories.AddRange(categories);
            await db.SaveChangesAsync();
        }

        // Products
        if (!await db.Products.AnyAsync())
        {
            var electronics = await db.Categories
                .FirstAsync(c => c.Slug == "electronics");
            var clothing = await db.Categories
                .FirstAsync(c => c.Slug == "clothing");
            var sports = await db.Categories
                .FirstAsync(c => c.Slug == "sports");

            var products = new List<Product>
            {
                new() {
                    Name = "iPhone 15 Pro",
                    Description = "Apple-ის უახლესი სმარტფონი A17 Pro ჩიპსეტით",
                    Price = 2999.99m,
                    StockQuantity = 50,
                    CategoryId = electronics.Id,
                    ImageUrl = "https://placehold.co/400x300?text=iPhone+15"
                },
                new() {
                    Name = "Samsung Galaxy S24",
                    Description = "Samsung-ის ფლაგმანი AI ფუნქციებით",
                    Price = 2499.99m,
                    StockQuantity = 30,
                    CategoryId = electronics.Id,
                    ImageUrl = "https://placehold.co/400x300?text=Galaxy+S24"
                },
                new() {
                    Name = "Sony WH-1000XM5",
                    Description = "უსადენო ყურსასმენი ხმაურის გამთიშველით",
                    Price = 899.99m,
                    StockQuantity = 100,
                    CategoryId = electronics.Id,
                    ImageUrl = "https://placehold.co/400x300?text=Sony+WH5"
                },
                new() {
                    Name = "MacBook Air M3",
                    Description = "Apple სილიკონის ლეპტოპი 15 საათის ბატარეით",
                    Price = 4599.99m,
                    StockQuantity = 20,
                    CategoryId = electronics.Id,
                    ImageUrl = "https://placehold.co/400x300?text=MacBook+Air"
                },
                new() {
                    Name = "Nike Air Max 270",
                    Description = "სპორტული ფეხსაცმელი მაქსიმალური კომფორტით",
                    Price = 349.99m,
                    StockQuantity = 200,
                    CategoryId = sports.Id,
                    ImageUrl = "https://placehold.co/400x300?text=Nike+Air+Max"
                },
                new() {
                    Name = "Adidas Ultraboost 23",
                    Description = "სარბენი ფეხსაცმელი Boost ტექნოლოგიით",
                    Price = 429.99m,
                    StockQuantity = 150,
                    CategoryId = sports.Id,
                    ImageUrl = "https://placehold.co/400x300?text=Adidas+UB23"
                },
                new() {
                    Name = "Levi's 501 ჯინსი",
                    Description = "კლასიკური straight fit ჯინსი",
                    Price = 189.99m,
                    StockQuantity = 300,
                    CategoryId = clothing.Id,
                    ImageUrl = "https://placehold.co/400x300?text=Levis+501"
                },
                new() {
                    Name = "The North Face ქურთუკი",
                    Description = "თბილი ზამთრის ქურთუკი",
                    Price = 599.99m,
                    StockQuantity = 80,
                    CategoryId = clothing.Id,
                    ImageUrl = "https://placehold.co/400x300?text=North+Face"
                },
            };
            db.Products.AddRange(products);
            await db.SaveChangesAsync();
        }

        // Admin User 
        if (!await db.Users.AnyAsync(u => u.Role == 0))
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hash = sha.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes("Admin123!" + "salt_2024"));

            db.Users.Add(new User
            {
                Email = "admin@eshop.ge",
                PasswordHash = Convert.ToBase64String(hash),
                FirstName = "Admin",
                LastName = "User",
                Role = (UserRole)1
            });
            await db.SaveChangesAsync();
        }
    }
}
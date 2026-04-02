using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


// Sample data and LINQ examples
// Initialize sample data locally using tuples (no type declarations in this file)
var products = new[]
{
    (Name: "Laptop", Category: "Electronics", Price: 1299.99m),
    (Name: "Headphones", Category: "Electronics", Price: 199.50m),
    (Name: "Coffee", Category: "Groceries", Price: 9.99m),
    (Name: "Desk Chair", Category: "Furniture", Price: 149.95m),
    (Name: "Office Desk", Category: "Furniture", Price: 349.00m),
    (Name: "Bananas", Category: "Groceries", Price: 2.49m)
};

// 1) Filter: expensive products over 200 ordered by price desc
var expensiveProducts = products
    .Where(p => p.Price > 200m)
    .OrderByDescending(p => p.Price)
    .ToList();

// 2) Projection: project to name and discounted price (10% off)
var discounted = products
    .Select(p => new { p.Name, DiscountedPrice = decimal.Round(p.Price * 0.9m, 2) })
    .ToList();

// 3) Grouping: group by category with count and average price
var byCategory = products
    .GroupBy(p => p.Category)
    .Select(g => new { Category = g.Key, Count = g.Count(), AveragePrice = decimal.Round(g.Average(p => p.Price), 2) })
    .ToList();

// Log results
app.Logger.LogInformation("Expensive products: {products}", string.Join(", ", expensiveProducts.Select(p => $"{p.Name} ({p.Price:C})")));
app.Logger.LogInformation("Discounted prices: {items}", string.Join(", ", discounted.Select(d => $"{d.Name} ({d.DiscountedPrice:C})")));
app.Logger.LogInformation("Products by category: {groups}", string.Join(", ", byCategory.Select(g => $"{g.Category}: {g.Count} items (avg {g.AveragePrice:C})")));

app.Run();

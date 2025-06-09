using OrderService.Data;
using OrderService.Models;

namespace OrderService.Services;

public static class DataSeeder
{
    public static void SeedDevelopmentData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        if (!db.Customers.Any())
        {
            var order = new Order
            {
                Customer = new Customer { Name = "Test User", Email = "test@example.com" },
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Items =
                [
                    new() { ProductName = "Test Order Item", UnitPrice = 9.99m, Quantity = 2 }
                ]
            };

            db.Orders.Add(order);
            Console.WriteLine(db.ChangeTracker.DebugView.ShortView);
            db.SaveChanges();
            Console.WriteLine($"Customer ID: {order.Customer.Id}");
            Console.WriteLine($"Order ID: {order.Id}");
            Console.WriteLine($"First OrderItem ID: {order.Items.First().Id}");
        }
    }
}

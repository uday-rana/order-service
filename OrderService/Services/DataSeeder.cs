using OrderService.Data;
using OrderService.Models;

namespace OrderService.Services;

public static class DataSeeder
{
    public static void SeedDevelopmentData(WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        OrderDbContext context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        if (!context.Customers.Any())
        {
            Customer[] customers =
            [
                new() { Name = "Alice Smith", Email = "alice@example.com" },
                new() { Name = "Bob Jones", Email = "bob@example.com" },
                new() { Name = "Charlie Brown", Email = "charlie@example.com" }
            ];

            Product[] products =
            [
                new() { Name = "Widget", Price = 19.99m },
                new() { Name = "Gadget", Price = 29.99m },
                new() { Name = "Thingamajig", Price = 14.99m }
            ];

            Order[] orders =
            [
                new()
                {
                    Customer = customers[0],
                    CreatedAt = new DateTime(2023, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    Status = OrderStatus.Fulfilled,
                    FulfilledAt = new DateTime(2023, 11, 5, 0, 0, 0, DateTimeKind.Utc),
                    Items =
                    [
                        new() { Product = products[0], Quantity = 1 },
                        new() { Product = products[1], Quantity = 2 }
                    ]
                },
                new()
                {
                    Customer = customers[1],
                    CreatedAt = new DateTime(2023, 12, 25, 0, 0, 0, DateTimeKind.Utc),
                    Status = OrderStatus.Cancelled,
                    Items =
                    [
                        new() { Product = products[2], Quantity = 4 }
                    ]
                },
                new()
                {
                    Customer = customers[2],
                    CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    Status = OrderStatus.Pending,
                    Items =
                    [
                        new() { Product = products[0], Quantity = 3 }
                    ]
                },
                new()
                {
                    Customer = customers[0],
                    CreatedAt = new DateTime(2024, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                    Status = OrderStatus.Fulfilled,
                    FulfilledAt = new DateTime(2024, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                    Items =
                    [
                        new() { Product = products[1], Quantity = 1 },
                        new() { Product = products[2], Quantity = 2 }
                    ]
                },
                new()
                {
                    Customer = customers[1],
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    Status = OrderStatus.Pending,
                    Items =
                    [
                        new() { Product = products[0], Quantity = 2 },
                        new() { Product = products[2], Quantity = 1 }
                    ]
                }
            ];

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}

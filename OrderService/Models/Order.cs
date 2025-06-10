namespace OrderService.Models;

public class Order
{
    public int Id { get; set; }
    public required ICollection<OrderItem> Items { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FulfilledAt { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public int CustomerId { get; set; }
    public required Customer Customer { get; set; }
}

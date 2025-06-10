namespace OrderService.Models;

public class OrderItem
{
    public int Id { get; set; }
    public required int Quantity { get; set; }
    public int ProductId { get; set; }
    public required Product Product { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}

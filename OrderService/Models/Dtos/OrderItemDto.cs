namespace OrderService.Models.Dtos;

public class OrderItemDto
{
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

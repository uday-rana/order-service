namespace OrderService.Models.Dtos;

public class OrderDto
{
    public int Id { get; set; }

    public CustomerDto Customer { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime? FulfilledAt { get; set; }

    public string Status { get; set; } = null!;
}

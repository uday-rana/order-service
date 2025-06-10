namespace OrderService.Models.Dtos;

public class OrderDto
{
    public int Id { get; set; }

    public CustomerDto Customer { get; set; } = null!;
    public List<ItemDto> Items { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime? FulfilledAt { get; set; }

    public string Status { get; set; } = null!;

    public class CustomerDto
    {
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
    }

    public class ItemDto
    {
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}

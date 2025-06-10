namespace OrderService.Models.Responses;

public class OrderResponse
{
    public int Id { get; set; }

    public CustomerResponse Customer { get; set; } = null!;
    public List<ItemResponse> Items { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime? FulfilledAt { get; set; }

    public string Status { get; set; } = null!;

    public class CustomerResponse
    {
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
    }

    public class ItemResponse
    {
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}

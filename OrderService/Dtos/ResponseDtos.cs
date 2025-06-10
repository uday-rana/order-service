namespace OrderService.Dtos;

public record OrderDto(
    int Id,
    CustomerDto Customer,
    IReadOnlyList<OrderItemDto> Items,
    DateTime CreatedAt,
    DateTime? FulfilledAt,
    string Status
);

public record CustomerDto(
    int Id,
    string Name,
    string Email
);

public record OrderItemDto(
    int Id,
    ProductDto Product,
    int Quantity
);

public record ProductDto(
    int Id,
    string Name,
    decimal Price
);

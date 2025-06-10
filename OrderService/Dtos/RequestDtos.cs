using OrderService.Models;

namespace OrderService.Dtos;

public record OrderCreateDto(
    int CustomerId,
    List<OrderItemCreateDto> Items
);

public record OrderItemCreateDto(
    int ProductId,
    int Quantity
);

public record OrderUpdateDto(
    OrderStatus Status
);

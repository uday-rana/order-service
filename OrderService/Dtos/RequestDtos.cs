using System.ComponentModel.DataAnnotations;

using OrderService.Models;

namespace OrderService.Dtos;

public record OrderCreateDto(
    [Required]
    int CustomerId,

    [Required]
    [MinLength(1)]
    List<OrderItemCreateDto> Items
);

public record OrderItemCreateDto(
    [Required]
    int ProductId,

    [Required]
    [Range(1, int.MaxValue)]
    int Quantity
);

public record OrderUpdateDto(
    [Required]
    OrderStatus Status
);

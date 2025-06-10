using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.Extensions
{
    public static class MappingExtensions
    {
        public static OrderDto ToDto(this Order o) => new(
            Id: o.Id,
            Customer: new CustomerDto(
                Id: o.Customer.Id,
                Name: o.Customer.Name,
                Email: o.Customer.Email
            ),
            Items: [.. o.Items.Select(i => new OrderItemDto(
                Id: i.Id,
                Product: new ProductDto(
                    Id: i.Product.Id,
                    Name: i.Product.Name,
                    Price: i.Product.Price
                ),
                Quantity: i.Quantity
            ))],
            CreatedAt: o.CreatedAt,
            FulfilledAt: o.FulfilledAt,
            Status: o.Status.ToString()
        );

        public static IEnumerable<OrderDto> ToDto(this IEnumerable<Order> orders)
            => orders.Select(o => o.ToDto());
    }
}

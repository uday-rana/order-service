using Microsoft.EntityFrameworkCore;

using OrderService.Data;
using OrderService.Interfaces;
using OrderService.Models.Dtos;

namespace OrderService.Services;

public class OrderService(OrderDbContext context) : IOrderService
{
    private readonly OrderDbContext _context = context;

    public async Task<List<OrderDto>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                FulfilledAt = o.FulfilledAt,
                Status = o.Status.ToString(),
                Customer = new OrderDto.CustomerDto
                {
                    Name = o.Customer.Name,
                    Email = o.Customer.Email
                },
                Items = o.Items.Select(i => new OrderDto.ItemDto
                {
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            })
            .ToListAsync();
    }
}

using Microsoft.EntityFrameworkCore;

using OrderService.Data;
using OrderService.Interfaces;
using OrderService.Models;
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
                Customer = new CustomerDto
                {
                    Name = o.Customer.Name,
                    Email = o.Customer.Email
                },
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order is null
            ? null
            : new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                FulfilledAt = order.FulfilledAt,
                Status = order.Status.ToString(),
                Customer = new CustomerDto
                {
                    Name = order.Customer.Name,
                    Email = order.Customer.Email
                },
                Items = [.. order.Items.Select(i => new OrderItemDto
                {
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                })]
            };
    }
}

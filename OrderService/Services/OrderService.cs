using Microsoft.EntityFrameworkCore;

using OrderService.Data;
using OrderService.Interfaces;
using OrderService.Models.Responses;

namespace OrderService.Services;

public class OrderService(OrderDbContext context) : IOrderService
{
    private readonly OrderDbContext _context = context;

    public async Task<List<OrderResponse>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Select(o => new OrderResponse
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                FulfilledAt = o.FulfilledAt,
                Status = o.Status.ToString(),
                Customer = new OrderResponse.CustomerResponse
                {
                    Name = o.Customer.Name,
                    Email = o.Customer.Email
                },
                Items = o.Items.Select(i => new OrderResponse.ItemResponse
                {
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            })
            .ToListAsync();
    }
}

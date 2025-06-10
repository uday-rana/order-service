using Microsoft.EntityFrameworkCore;

using OrderService.Data;
using OrderService.Dtos;
using OrderService.Extensions;
using OrderService.Interfaces;
using OrderService.Models;

namespace OrderService.Services;

public class OrderService(OrderDbContext context) : IOrderService
{
    private readonly OrderDbContext _context = context;

    public async Task<List<OrderDto>> GetAllAsync() => await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Select(o => o.ToDto())
            .ToListAsync();

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        Order? order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order?.ToDto();
    }

    public async Task<OrderDto> CreateAsync(OrderCreateDto dto)
    {
        Customer customer = await _context.Customers.FindAsync(dto.CustomerId)
            ?? throw new Exception("Customer not found");

        // Get the list of product ids from the dto
        IEnumerable<int> productIds = dto.Items
            .Select(i => i.ProductId)
            .Distinct();

        // Get the product entities using the ids and store them in a dict with the ids as the keys
        Dictionary<int, Product> products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        Order newOrder = new()
        {
            Customer = customer,
            Items = [.. dto.Items.Select(i => new OrderItem
                {
                    Product = products[i.ProductId],
                    Quantity = i.Quantity
                })]
        };

        _context.Orders.Add(newOrder);
        await _context.SaveChangesAsync();

        return newOrder.ToDto();
    }

    public async Task<OrderDto?> UpdateAsync(int id, OrderUpdateDto dto)
    {
        Order? order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) { return null; }


        order.Status = dto.Status;
        if (order.Status == OrderStatus.Fulfilled)
        {
            order.FulfilledAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return order.ToDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Order? order = await _context.Orders.FindAsync(id);

        if (order is null) { return false; }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }
}

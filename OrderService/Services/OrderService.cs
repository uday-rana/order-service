using Microsoft.EntityFrameworkCore;

using OrderService.Data;
using OrderService.Dtos;
using OrderService.Extensions;
using OrderService.Interfaces;
using OrderService.Models;

namespace OrderService.Services;

public class OrderService(OrderDbContext context, ILogger<OrderService> logger) : IOrderService
{
    private readonly OrderDbContext _context = context;
    private readonly ILogger<OrderService> _logger = logger;

    public async Task<List<OrderDto>> GetAllAsync()
    {
        List<OrderDto> orders = await _context.Orders
        .Include(o => o.Customer)
        .Include(o => o.Items)
        .ThenInclude(i => i.Product)
        .Select(o => o.ToDto())
        .ToListAsync();

        _logger.LogInformation("Fetched {OrderCount} order(s)", orders.Count);

        return orders;
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        Order? order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found", id);
            return null;
        }

        _logger.LogInformation("Fetched order {OrderId}", id);

        return order.ToDto();
    }

    public async Task<OrderDto> CreateAsync(OrderCreateDto dto)
    {
        Customer? customer = await _context.Customers.FindAsync(dto.CustomerId);

        if (customer is null)
        {
            _logger.LogWarning("Order creation failed: Customer {CustomerId} not found", dto.CustomerId);
            throw new BadHttpRequestException("Customer not found");
        }

        // Get the list of product ids from the dto
        IEnumerable<int> productIds = dto.Items
            .Select(i => i.ProductId)
            .Distinct();

        // Get the product entities using the ids and store them in a dictionary with the ids as the keys
        Dictionary<int, Product> products = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        // Validate that all of the referenced products exist
        IEnumerable<int> missingProductIds = productIds.Where(id => !products.ContainsKey(id));
        if (missingProductIds.Any())
        {
            string missingList = string.Join(", ", missingProductIds);
            _logger.LogWarning(
                "Order creation failed: Product(s) {MissingProductIds} not found",
                missingList);
            throw new BadHttpRequestException("Product not found");
        }

        Order order = new()
        {
            Customer = customer,
            Items = [.. dto.Items.Select(i => new OrderItem
            {
                Product = products[i.ProductId],
                Quantity = i.Quantity
            })]
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} created", order.Id);

        return order.ToDto();
    }

    public async Task<OrderDto?> UpdateAsync(int id, OrderUpdateDto dto)
    {
        Order? order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            _logger.LogWarning("Order update failed: Order {OrderId} not found", id);
            return null;
        }


        order.Status = dto.Status;
        if (order.Status == OrderStatus.Fulfilled)
        {
            order.FulfilledAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} updated", order.Id);

        return order.ToDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Order? order = await _context.Orders.FindAsync(id);

        if (order is null) { return false; }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Order {OrderId} deleted", order.Id);

        return true;
    }
}

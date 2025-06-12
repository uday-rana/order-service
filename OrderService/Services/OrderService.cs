using Microsoft.AspNetCore.Mvc;
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

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync([FromQuery] OrderQueryParameters query)
    {
        IQueryable<Order> orders = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product);

        if (query.CustomerId.HasValue)
        {
            orders = orders.Where(o => o.CustomerId == query.CustomerId.Value);
        }

        if (query.Status.HasValue)
        {
            orders = orders.Where(o => o.Status == query.Status);
        }

        if (query.CreatedFrom.HasValue)
        {
            DateTime from = query.CreatedFrom.Value;
            orders = orders.Where(o => o.CreatedAt >= from);
        }

        if (query.CreatedTo.HasValue)
        {
            DateTime to = query.CreatedTo.Value;
            orders = orders.Where(o => o.CreatedAt <= to);
        }

        if (query.FulfilledFrom.HasValue)
        {
            DateTime from = query.FulfilledFrom.Value;
            orders = orders.Where(o => o.FulfilledAt.HasValue && o.FulfilledAt.Value >= from);
        }

        if (query.FulfilledTo.HasValue)
        {
            DateTime to = query.FulfilledTo.Value;
            orders = orders.Where(o => o.FulfilledAt.HasValue && o.FulfilledAt.Value <= to);
        }

        if (query.ProductId.HasValue)
        {
            int productId = query.ProductId.Value;
            orders = orders.Where(o => o.Items.Any(i => i.ProductId == productId));
        }

        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            bool desc = query.SortDescending;
            string sort = query.SortBy!.ToLowerInvariant();
            orders = sort switch
            {
                "createdat" => desc ? orders.OrderByDescending(o => o.CreatedAt) : orders.OrderBy(o => o.CreatedAt),
                "fulfilledat" => desc ? orders.OrderByDescending(o => o.FulfilledAt) : orders.OrderBy(o => o.FulfilledAt),
                "status" => desc ? orders.OrderByDescending(o => o.Status) : orders.OrderBy(o => o.Status),
                _ => desc ? orders.OrderByDescending(o => o.Id) : orders.OrderBy(o => o.Id)
            };
        }
        else
        {
            orders = query.SortDescending ? orders.OrderByDescending(o => o.Id) : orders.OrderBy(o => o.Id);
        }

        orders = orders
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);

        List<OrderDto> result = await orders
            .Select(o => o.ToDto())
            .ToListAsync();

        _logger.LogInformation("Fetched {OrderCount} order(s)", result.Count);

        return result;
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

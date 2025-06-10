using OrderService.Models.Dtos;

namespace OrderService.Interfaces;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync();
}

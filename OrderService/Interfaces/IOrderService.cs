using OrderService.Models.Responses;

namespace OrderService.Interfaces;

public interface IOrderService
{
    Task<List<OrderResponse>> GetAllAsync();
}

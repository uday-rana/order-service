using OrderService.Dtos;

namespace OrderService.Interfaces;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync();
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto> CreateAsync(OrderCreateDto dto);
    Task<OrderDto?> UpdateAsync(int id, OrderUpdateDto dto);
}

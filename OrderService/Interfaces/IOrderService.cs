using OrderService.Dtos;

namespace OrderService.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetAllAsync(OrderQueryParameters query);
    Task<OrderDto?> GetByIdAsync(int id);
    Task<OrderDto> CreateAsync(OrderCreateDto dto);
    Task<OrderDto?> UpdateAsync(int id, OrderUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}

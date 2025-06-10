using Microsoft.AspNetCore.Mvc;

using OrderService.Dtos;
using OrderService.Interfaces;

namespace OrderService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController(IOrderService service) : ControllerBase
{
    private readonly IOrderService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        List<OrderDto> orders = await _service.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        OrderDto? order = await _service.GetByIdAsync(id);
        return order is null ? NotFound() : Ok(order);
    }
}

using Microsoft.AspNetCore.Mvc;

using OrderService.Interfaces;
using OrderService.Models.Dtos;

namespace OrderService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController(IOrderService service) : ControllerBase
{
    private readonly IOrderService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var orders = await _service.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _service.GetByIdAsync(id);
        return order is null ? NotFound() : Ok(order);
    }
}

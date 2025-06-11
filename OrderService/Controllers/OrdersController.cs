using Microsoft.AspNetCore.Authorization;
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

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<OrderDto>> Create(OrderCreateDto dto)
    {
        try
        {
            OrderDto created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(new ProblemDetails { Title = "Bad Request", Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1", Detail = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    [Authorize]
    public async Task<ActionResult<OrderDto>> Update(int id, OrderUpdateDto dto)
    {
        OrderDto? updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        bool deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

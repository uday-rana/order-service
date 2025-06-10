using Microsoft.AspNetCore.Mvc;

using OrderService.Interfaces;
using OrderService.Models.Responses;

namespace OrderService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController(IOrderService service) : ControllerBase
{
    private readonly IOrderService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetAll()
    {
        var orders = await _service.GetAllAsync();
        return Ok(orders);
    }
}

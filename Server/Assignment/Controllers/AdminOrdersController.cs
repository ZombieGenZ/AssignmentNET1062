using Assignment.Dtos.Orders;
using Assignment.Enums;
using Assignment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdminOrderListItemDto>>> GetOrders([FromQuery] OrderStatus? status)
        {
            var orders = await _orderService.GetOrdersAsync(status);
            return Ok(orders);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrder(Guid id)
        {
            var order = await _orderService.GetOrderDetailForAdminAsync(id);
            if (order == null) return NotFound();

            return Ok(order);
        }

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderStatus status)
        {
            var updated = await _orderService.UpdateOrderStatusAsync(id, status);
            if (!updated) return NotFound();

            return NoContent();
        }
    }
}

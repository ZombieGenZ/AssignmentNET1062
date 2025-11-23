using Assignment.Dtos.Orders;
using Assignment.Dtos.Vouchers;
using Assignment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IVoucherService _voucherService;

        public OrdersController(IOrderService orderService, IVoucherService voucherService)
        {
            _orderService = orderService;
            _voucherService = voucherService;
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<CheckoutResponse>> Checkout([FromBody] CheckoutRequest request)
        {
            var result = await _orderService.CheckoutAsync(User, request);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<OrderListItemDto>>> MyOrders()
        {
            var list = await _orderService.GetMyOrdersAsync(User);
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrder(Guid id)
        {
            var order = await _orderService.GetOrderDetailAsync(User, id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost("validate-voucher")]
        public async Task<ActionResult<ValidateVoucherResponse>> ValidateVoucher([FromBody] ValidateVoucherRequest request)
        {
            var result = await _voucherService.ValidateVoucherAsync(request);
            return Ok(result);
        }
    }
}

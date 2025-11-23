using Assignment.Dtos.Cart;
using Assignment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService) { _cartService = cartService; }

        [HttpGet]
        public async Task<ActionResult<CartResponse>> GetCart()
        {
            var cart = await _cartService.GetCartAsync(User);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request)
        {
            await _cartService.AddItemAsync(User, request);
            return NoContent();
        }

        [HttpPut("items/{id:guid}")]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateCartItemRequest request)
        {
            await _cartService.UpdateItemAsync(User, id, request);
            return NoContent();
        }

        [HttpDelete("items/{id:guid}")]
        public async Task<IActionResult> RemoveItem(Guid id)
        {
            await _cartService.RemoveItemAsync(User, id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync(User);
            return NoContent();
        }
    }
}

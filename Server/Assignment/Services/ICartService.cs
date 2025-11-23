using Assignment.Dtos.Cart;
using System.Security.Claims;

namespace Assignment.Services
{
    public interface ICartService
    {
        Task<CartResponse> GetCartAsync(ClaimsPrincipal principal);
        Task AddItemAsync(ClaimsPrincipal principal, AddCartItemRequest request);
        Task UpdateItemAsync(ClaimsPrincipal principal, Guid id, UpdateCartItemRequest request);
        Task RemoveItemAsync(ClaimsPrincipal principal, Guid id);
        Task ClearCartAsync(ClaimsPrincipal principal);
    }
}

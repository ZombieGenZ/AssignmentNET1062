using Assignment.Dtos.Orders;
using System.Security.Claims;

namespace Assignment.Services
{
    public interface IOrderService
    {
        Task<CheckoutResponse> CheckoutAsync(ClaimsPrincipal principal, CheckoutRequest request);
        Task<List<OrderListItemDto>> GetMyOrdersAsync(ClaimsPrincipal principal);
        Task<OrderDetailDto?> GetOrderDetailAsync(ClaimsPrincipal principal, Guid id);
    }
}

using Assignment.Dtos.Orders;
using Assignment.Enums;
using System.Security.Claims;

namespace Assignment.Services
{
    public interface IOrderService
    {
        Task<CheckoutResponse> CheckoutAsync(ClaimsPrincipal principal, CheckoutRequest request);
        Task<List<OrderListItemDto>> GetMyOrdersAsync(ClaimsPrincipal principal);
        Task<OrderDetailDto?> GetOrderDetailAsync(ClaimsPrincipal principal, Guid id);
        Task<List<AdminOrderListItemDto>> GetOrdersAsync(OrderStatus? status);
        Task<OrderDetailDto?> GetOrderDetailForAdminAsync(Guid id);
        Task<bool> UpdateOrderStatusAsync(Guid id, OrderStatus status);
    }
}

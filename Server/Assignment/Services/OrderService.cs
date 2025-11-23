using Assignment.Data;
using Assignment.Dtos.Orders;
using Assignment.Dtos.Vouchers;
using Assignment.Enums;
using Assignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Assignment.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IVoucherService _voucherService;

        public OrderService(AppDbContext db, UserManager<AppUser> userManager, IVoucherService voucherService)
        {
            _db = db;
            _userManager = userManager;
            _voucherService = voucherService;
        }

        private async Task<AppUser> GetUserAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null) throw new ApplicationException("User không tồn tại.");
            return user;
        }

        public async Task<CheckoutResponse> CheckoutAsync(ClaimsPrincipal principal, CheckoutRequest request)
        {
            var user = await GetUserAsync(principal);
            var cart = await _db.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null) throw new ApplicationException("Giỏ hàng trống.");

            var selectedItems = cart.Items.Where(i => request.CartItemIds.Contains(i.Id)).ToList();
            if (!selectedItems.Any()) throw new ApplicationException("Không có sản phẩm nào để thanh toán.");

            decimal subtotal = 0;

            foreach (var item in selectedItems)
            {
                if (item.ItemType == CartItemType.Product && item.ProductId.HasValue)
                {
                    var product = await _db.Products.FindAsync(item.ProductId.Value);
                    if (product != null)
                        subtotal += product.Price * item.Quantity;
                }
                else if (item.ItemType == CartItemType.Combo && item.ComboId.HasValue)
                {
                    var combo = await _db.Combos.Include(c => c.Items).ThenInclude(i => i.Product)
                        .FirstOrDefaultAsync(c => c.Id == item.ComboId.Value);

                    if (combo != null)
                    {
                        var original = combo.Items.Sum(i => i.Product.Price * i.Quantity);
                        var final = original - original * (combo.DiscountPercent / 100m);
                        subtotal += final * item.Quantity;
                    }
                }
            }

            decimal discount = 0;
            Voucher? voucher = null;

            if (!string.IsNullOrWhiteSpace(request.VoucherCode))
            {
                var validate = await _voucherService.ValidateVoucherAsync(new ValidateVoucherRequest
                {
                    Code = request.VoucherCode,
                    OrderAmount = subtotal
                });

                if (validate.IsValid)
                {
                    discount = validate.DiscountAmount;
                    voucher = await _db.Vouchers.FirstOrDefaultAsync(v => v.Code == request.VoucherCode);
                }
            }

            var total = subtotal - discount;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CustomerName = request.CustomerName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
                PaymentMethod = request.PaymentMethod,
                PayNowGateway = request.PaymentMethod == PaymentMethod.PayNow ? request.PayNowGateway.GetValueOrDefault() : PayNowGateway.None,
                VoucherId = voucher?.Id,
                TotalPrice = total,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in selectedItems)
            {
                if (item.ItemType == CartItemType.Product && item.ProductId.HasValue)
                {
                    var product = await _db.Products.FindAsync(item.ProductId.Value);
                    if (product == null) continue;

                    order.Items.Add(new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ItemType = OrderItemType.Product,
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });
                }
                else if (item.ItemType == CartItemType.Combo && item.ComboId.HasValue)
                {
                    var combo = await _db.Combos.Include(c => c.Items).ThenInclude(i => i.Product)
                        .FirstOrDefaultAsync(c => c.Id == item.ComboId.Value);
                    if (combo == null) continue;

                    var original = combo.Items.Sum(i => i.Product.Price * i.Quantity);
                    var final = original - original * (combo.DiscountPercent / 100m);

                    order.Items.Add(new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ItemType = OrderItemType.Combo,
                        ComboId = combo.Id,
                        Quantity = item.Quantity,
                        UnitPrice = final
                    });
                }
            }

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(selectedItems);
            await _db.SaveChangesAsync();

            // demo: không tích hợp cổng thanh toán thật
            return new CheckoutResponse
            {
                OrderId = order.Id,
                PaymentUrl = null
            };
        }

        public async Task<List<OrderListItemDto>> GetMyOrdersAsync(ClaimsPrincipal principal)
        {
            var user = await GetUserAsync(principal);
            return await _db.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderListItemDto
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status
                }).ToListAsync();
        }

        public async Task<OrderDetailDto?> GetOrderDetailAsync(ClaimsPrincipal principal, Guid id)
        {
            var user = await GetUserAsync(principal);
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null) return null;

            var dto = new OrderDetailDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                CustomerName = order.CustomerName,
                PhoneNumber = order.PhoneNumber,
                Email = order.Email,
                Address = order.Address,
                PaymentMethod = order.PaymentMethod,
                PayNowGateway = order.PayNowGateway,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                VoucherCode = order.Voucher?.Code,
                Items = new List<OrderItemDto>()
            };

            foreach (var item in order.Items)
            {
                string name;
                if (item.ItemType == OrderItemType.Product && item.ProductId.HasValue)
                {
                    var product = await _db.Products.FindAsync(item.ProductId.Value);
                    name = product?.Name ?? "Sản phẩm";
                }
                else if (item.ItemType == OrderItemType.Combo && item.ComboId.HasValue)
                {
                    var combo = await _db.Combos.FindAsync(item.ComboId.Value);
                    name = combo?.Name ?? "Combo";
                }
                else
                {
                    name = "Unknown";
                }

                dto.Items.Add(new OrderItemDto
                {
                    Id = item.Id,
                    ItemType = item.ItemType,
                    Name = name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            return dto;
        }
    }
}

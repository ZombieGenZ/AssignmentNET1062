using Assignment.Data;
using Assignment.Dtos.Cart;
using Assignment.Enums;
using Assignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Assignment.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public CartService(AppDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private async Task<Cart> GetOrCreateCartAsync(AppUser user)
        {
            var cart = await _db.Carts.Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .Include(c => c.Items)
                .ThenInclude(i => i.Combo)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new Cart { Id = Guid.NewGuid(), UserId = user.Id };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
                cart = await _db.Carts.Include(c => c.Items)
                    .FirstAsync(c => c.Id == cart.Id);
            }

            return cart;
        }

        private async Task<AppUser> GetUserAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null) throw new ApplicationException("User không tồn tại.");
            return user;
        }

        public async Task<CartResponse> GetCartAsync(ClaimsPrincipal principal)
        {
            var user = await GetUserAsync(principal);
            var cart = await GetOrCreateCartAsync(user);

            await _db.Entry(cart).Collection(c => c.Items).LoadAsync();
            await _db.Entry(cart).Collection(c => c.Items).Query().Include(i => i.Product).Include(i => i.Combo).LoadAsync();

            var dto = new CartResponse
            {
                Items = cart.Items.Select(i =>
                {
                    decimal unitPrice;
                    string name;
                    string? imageUrl;

                    if (i.ItemType == CartItemType.Product && i.Product != null)
                    {
                        unitPrice = i.Product.Price;
                        name = i.Product.Name;
                        imageUrl = i.Product.ImageUrl;
                    }
                    else if (i.ItemType == CartItemType.Combo && i.Combo != null)
                    {
                        var original = _db.ComboItems.Where(x => x.ComboId == i.ComboId)
                            .Include(x => x.Product)
                            .Sum(x => x.Product.Price * x.Quantity);

                        unitPrice = original - original * (i.Combo!.DiscountPercent / 100m);
                        name = i.Combo.Name;
                        imageUrl = i.Combo.ImageUrl;
                    }
                    else
                    {
                        unitPrice = 0;
                        name = "Unknown";
                        imageUrl = null;
                    }

                    return new CartItemDto
                    {
                        Id = i.Id,
                        ItemType = i.ItemType,
                        ProductId = i.ProductId,
                        ComboId = i.ComboId,
                        Name = name,
                        ImageUrl = imageUrl,
                        Quantity = i.Quantity,
                        UnitPrice = unitPrice
                    };
                }).ToList()
            };

            return dto;
        }

        public async Task AddItemAsync(ClaimsPrincipal principal, AddCartItemRequest request)
        {
            var user = await GetUserAsync(principal);
            var cart = await GetOrCreateCartAsync(user);

            if (request.Quantity <= 0) request.Quantity = 1;

            if (request.ItemType == CartItemType.Product)
            {
                var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId && p.IsActive);
                if (product == null)
                {
                    throw new ApplicationException("Sản phẩm không tồn tại hoặc đã bị ẩn.");
                }
            }
            else if (request.ItemType == CartItemType.Combo)
            {
                var combo = await _db.Combos.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == request.ComboId && c.IsActive);
                if (combo == null || !combo.Items.Any())
                {
                    throw new ApplicationException("Combo không hợp lệ hoặc đã ngừng bán.");
                }
            }

            var item = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ItemType = request.ItemType,
                ProductId = request.ItemType == CartItemType.Product ? request.ProductId : null,
                ComboId = request.ItemType == CartItemType.Combo ? request.ComboId : null,
                Quantity = request.Quantity
            };

            _db.CartItems.Add(item);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(ClaimsPrincipal principal, Guid id, UpdateCartItemRequest request)
        {
            var user = await GetUserAsync(principal);
            var cart = await GetOrCreateCartAsync(user);

            var item = await _db.CartItems.FirstOrDefaultAsync(i => i.Id == id && i.CartId == cart.Id);
            if (item == null) throw new ApplicationException("Item không tồn tại.");

            if (request.Quantity <= 0)
            {
                _db.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = request.Quantity;
            }

            await _db.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(ClaimsPrincipal principal, Guid id)
        {
            var user = await GetUserAsync(principal);
            var cart = await GetOrCreateCartAsync(user);

            var item = await _db.CartItems.FirstOrDefaultAsync(i => i.Id == id && i.CartId == cart.Id);
            if (item == null) return;

            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task ClearCartAsync(ClaimsPrincipal principal)
        {
            var user = await GetUserAsync(principal);
            var cart = await GetOrCreateCartAsync(user);

            _db.CartItems.RemoveRange(cart.Items);
            await _db.SaveChangesAsync();
        }
    }
}

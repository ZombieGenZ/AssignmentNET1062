using Assignment.Data;
using Assignment.Dtos.Products;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Services
{
    public class ComboService : IComboService
    {
        private readonly AppDbContext _db;

        public ComboService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ComboListItemDto>> GetCombosAsync(string? search)
        {
            var query = _db.Combos.Include(c => c.Items).ThenInclude(i => i.Product)
                .Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Name.Contains(search));

            var list = await query.ToListAsync();

            return list.Select(c =>
            {
                var original = c.Items.Sum(i => i.Product.Price * i.Quantity);
                var final = original - original * (c.DiscountPercent / 100m);
                return new ComboListItemDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl,
                    FinalPrice = final
                };
            }).ToList();
        }

        public async Task<ComboDetailDto?> GetComboAsync(Guid id)
        {
            var combo = await _db.Combos
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (combo == null) return null;

            var original = combo.Items.Sum(i => i.Product.Price * i.Quantity);
            var final = original - original * (combo.DiscountPercent / 100m);

            return new ComboDetailDto
            {
                Id = combo.Id,
                Name = combo.Name,
                Description = combo.Description,
                ImageUrl = combo.ImageUrl,
                DiscountPercent = combo.DiscountPercent,
                OriginalPrice = original,
                FinalPrice = final,
                Items = combo.Items.Select(i => new ComboItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.Product.Price
                }).ToList()
            };
        }
    }
}

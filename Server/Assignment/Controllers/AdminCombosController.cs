using Assignment.Data;
using Assignment.Dtos.Products;
using Assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/admin/combos")]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminCombosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminCombosController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var combos = await _db.Combos.Include(c => c.Items).ToListAsync();
            var result = combos.Select(c => new ComboAdminListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                DiscountPercent = c.DiscountPercent,
                IsActive = c.IsActive,
                ItemCount = c.Items.Count
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var combo = await _db.Combos
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null) return NotFound();

            return Ok(MapToDetail(combo));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ComboUpsertDto model)
        {
            var combo = new Combo
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                DiscountPercent = model.DiscountPercent,
                IsActive = model.IsActive
            };

            _db.Combos.Add(combo);
            await _db.SaveChangesAsync();

            await UpsertItems(combo, model.Items);

            await _db.SaveChangesAsync();

            await _db.Entry(combo).Collection(c => c.Items).LoadAsync();
            await _db.Entry(combo).Collection(c => c.Items).Query().Include(i => i.Product).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = combo.Id }, MapToDetail(combo));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ComboUpsertDto model)
        {
            var combo = await _db.Combos
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (combo == null) return NotFound();

            combo.Name = model.Name;
            combo.Description = model.Description;
            combo.ImageUrl = model.ImageUrl;
            combo.DiscountPercent = model.DiscountPercent;
            combo.IsActive = model.IsActive;

            _db.ComboItems.RemoveRange(combo.Items);
            await _db.SaveChangesAsync();

            await UpsertItems(combo, model.Items);

            await _db.SaveChangesAsync();

            await _db.Entry(combo).Collection(c => c.Items).Query().Include(i => i.Product).LoadAsync();

            return Ok(MapToDetail(combo));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var combo = await _db.Combos.FindAsync(id);
            if (combo == null) return NotFound();

            combo.IsActive = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private async Task UpsertItems(Combo combo, List<ComboUpsertItemDto> items)
        {
            if (items == null || items.Count == 0) return;

            var validProducts = await _db.Products
                .Where(p => items.Select(i => i.ProductId).Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            foreach (var item in items)
            {
                if (!validProducts.Contains(item.ProductId) || item.Quantity <= 0) continue;

                _db.ComboItems.Add(new ComboItem
                {
                    ComboId = combo.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }
        }

        private static ComboDetailDto MapToDetail(Combo combo)
        {
            var original = combo.Items.Sum(i => i.Product.Price * i.Quantity);
            var final = original - original * (combo.DiscountPercent / 100m);

            return new ComboDetailDto
            {
                Id = combo.Id,
                Name = combo.Name,
                Description = combo.Description,
                ImageUrl = combo.ImageUrl,
                DiscountPercent = combo.DiscountPercent,
                IsActive = combo.IsActive,
                OriginalPrice = original,
                FinalPrice = final,
                Items = combo.Items.Select(i => new ComboItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.Product?.Price ?? 0
                }).ToList()
            };
        }
    }
}

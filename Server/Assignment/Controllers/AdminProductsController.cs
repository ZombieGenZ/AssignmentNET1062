using Assignment.Data;
using Assignment.Dtos.Products;
using Assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/admin/products")]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminProductsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminProductsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _db.Products.Include(p => p.Category).ToListAsync();
            var result = products.Select(MapToDto).ToList();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            return Ok(MapToDto(product));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductUpsertDto model)
        {
            var category = await _db.Categories.FindAsync(model.CategoryId);
            if (category == null) return BadRequest("Danh mục không tồn tại.");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                IsActive = model.IsActive
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            var dto = MapToDto(product, category.Name);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpsertDto model)
        {
            var product = await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var category = await _db.Categories.FindAsync(model.CategoryId);
            if (category == null) return BadRequest("Danh mục không tồn tại.");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;

            await _db.SaveChangesAsync();

            return Ok(MapToDto(product, category.Name));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            var linkedCombos = await _db.ComboItems
                .Include(ci => ci.Combo)
                .Where(ci => ci.ProductId == id && ci.Combo != null)
                .Select(ci => new { ci.Combo!.Id, ci.Combo.Name })
                .Distinct()
                .ToListAsync();

            if (linkedCombos.Any())
            {
                return Conflict(new
                {
                    message = "Sản phẩm đang thuộc các combo. Vui lòng chỉnh sửa combo trước khi xoá.",
                    combos = linkedCombos
                });
            }

            product.IsActive = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private static ProductAdminDto MapToDto(Product p, string? categoryName = null)
        {
            return new ProductAdminDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = categoryName ?? p.Category?.Name,
                IsActive = p.IsActive
            };
        }
    }
}

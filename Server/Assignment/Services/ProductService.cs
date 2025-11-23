using Assignment.Data;
using Assignment.Dtos.Products;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ProductListItemDto>> GetProductsAsync(string? search, Guid? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _db.Products.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                ImageUrl = p.ImageUrl,
                Price = p.Price
            }).ToListAsync();
        }

        public async Task<ProductDetailDto?> GetProductAsync(Guid id)
        {
            return await _db.Products.Where(p => p.Id == id && p.IsActive)
                .Select(p => new ProductDetailDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    Description = p.Description,
                    CategoryId = p.CategoryId
                }).FirstOrDefaultAsync();
        }
    }
}

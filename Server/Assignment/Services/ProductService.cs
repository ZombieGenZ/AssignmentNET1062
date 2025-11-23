using Assignment.Data;
using Assignment.Dtos.Products;
using Assignment.Models;
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

        public async Task<ProductDto> CreateAsync(ProductDto request)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = request.CategoryId,
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                IsActive = request.IsActive
            };

            foreach (var typeDto in request.ProductTypes)
            {
                ValidateType(typeDto);
                product.ProductTypes.Add(new ProductType
                {
                    Id = typeDto.Id ?? Guid.NewGuid(),
                    Name = typeDto.Name,
                    Description = typeDto.Description,
                    Price = typeDto.Price,
                    Stock = typeDto.Stock,
                    Sold = typeDto.Sold,
                    IsPublished = typeDto.IsPublished,
                    IsSpicy = typeDto.IsSpicy,
                    IsVegetarian = typeDto.IsVegetarian,
                    SortOrder = typeDto.SortOrder
                });
            }

            var extraIds = request.ExtraIds.Distinct().ToList();
            if (extraIds.Count > 0)
            {
                var extras = await _db.ProductExtras.Where(pe => extraIds.Contains(pe.Id) && pe.IsActive).ToListAsync();
                foreach (var extra in extras)
                {
                    product.ProductExtraProducts.Add(new ProductExtraProduct
                    {
                        ProductId = product.Id,
                        ProductExtraId = extra.Id,
                        Product = product,
                        ProductExtra = extra
                    });
                }
            }

            product.RefreshDerivedFields();

            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            await _db.Entry(product).Collection(p => p.ProductTypes).LoadAsync();
            await _db.Entry(product).Collection(p => p.ProductExtraProducts).LoadAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _db.Products
                .Include(p => p.ProductTypes)
                .Include(p => p.ProductExtraProducts)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto?> UpdateAsync(Guid id, ProductDto request)
        {
            var product = await _db.Products
                .Include(p => p.ProductTypes)
                .Include(p => p.ProductExtraProducts)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return null;

            product.CategoryId = request.CategoryId;
            product.Name = request.Name;
            product.Description = request.Description;
            product.ImageUrl = request.ImageUrl;
            product.IsActive = request.IsActive;

            // Sync product types
            var incomingTypeIds = request.ProductTypes.Where(t => t.Id.HasValue).Select(t => t.Id!.Value).ToHashSet();
            var removedTypes = product.ProductTypes.Where(t => !incomingTypeIds.Contains(t.Id)).ToList();
            foreach (var removed in removedTypes)
            {
                product.ProductTypes.Remove(removed);
                _db.ProductTypes.Remove(removed);
            }

            foreach (var typeDto in request.ProductTypes)
            {
                ValidateType(typeDto);
                var existing = typeDto.Id.HasValue
                    ? product.ProductTypes.FirstOrDefault(t => t.Id == typeDto.Id.Value)
                    : null;

                if (existing != null)
                {
                    existing.Name = typeDto.Name;
                    existing.Description = typeDto.Description;
                    existing.Price = typeDto.Price;
                    existing.Stock = typeDto.Stock;
                    existing.Sold = typeDto.Sold;
                    existing.IsPublished = typeDto.IsPublished;
                    existing.IsSpicy = typeDto.IsSpicy;
                    existing.IsVegetarian = typeDto.IsVegetarian;
                    existing.SortOrder = typeDto.SortOrder;
                }
                else
                {
                    product.ProductTypes.Add(new ProductType
                    {
                        Id = typeDto.Id ?? Guid.NewGuid(),
                        ProductId = product.Id,
                        Name = typeDto.Name,
                        Description = typeDto.Description,
                        Price = typeDto.Price,
                        Stock = typeDto.Stock,
                        Sold = typeDto.Sold,
                        IsPublished = typeDto.IsPublished,
                        IsSpicy = typeDto.IsSpicy,
                        IsVegetarian = typeDto.IsVegetarian,
                        SortOrder = typeDto.SortOrder
                    });
                }
            }

            // Sync extras
            var existingExtras = product.ProductExtraProducts.ToList();
            _db.ProductExtraProducts.RemoveRange(existingExtras);
            product.ProductExtraProducts.Clear();
            var extraIds = request.ExtraIds.Distinct().ToList();
            if (extraIds.Count > 0)
            {
                var extras = await _db.ProductExtras.Where(pe => extraIds.Contains(pe.Id) && pe.IsActive).ToListAsync();
                foreach (var extra in extras)
                {
                    product.ProductExtraProducts.Add(new ProductExtraProduct
                    {
                        ProductId = product.Id,
                        ProductExtraId = extra.Id,
                        Product = product,
                        ProductExtra = extra
                    });
                }
            }

            product.RefreshDerivedFields();
            await _db.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _db.Products
                .Include(p => p.ProductTypes)
                .Include(p => p.ProductExtraProducts)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return false;

            _db.ProductTypes.RemoveRange(product.ProductTypes);
            _db.ProductExtraProducts.RemoveRange(product.ProductExtraProducts);
            _db.Products.Remove(product);

            await _db.SaveChangesAsync();
            return true;
        }

        private static void ValidateType(ProductTypeDto dto)
        {
            if (dto.Price < 0)
                throw new ArgumentException("Price must be non-negative", nameof(dto.Price));
            if (dto.Stock < 0)
                throw new ArgumentException("Stock must be non-negative", nameof(dto.Stock));
            if (dto.Sold < 0)
                throw new ArgumentException("Sold must be non-negative", nameof(dto.Sold));
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                IsSpicy = product.IsSpicy,
                IsVegetarian = product.IsVegetarian,
                PriceMin = product.PriceMin,
                PriceMax = product.PriceMax,
                TotalStock = product.TotalStock,
                TotalSold = product.TotalSold,
                PrimaryProductTypeId = product.PrimaryProductTypeId,
                ProductTypes = product.ProductTypes
                    .OrderBy(t => t.SortOrder)
                    .ThenBy(t => t.Price)
                    .Select(t => new ProductTypeDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        Price = t.Price,
                        Stock = t.Stock,
                        Sold = t.Sold,
                        IsPublished = t.IsPublished,
                        IsSpicy = t.IsSpicy,
                        IsVegetarian = t.IsVegetarian,
                        SortOrder = t.SortOrder
                    }).ToList(),
                ExtraIds = product.ProductExtraProducts.Select(x => x.ProductExtraId).ToList()
            };
        }
    }
}

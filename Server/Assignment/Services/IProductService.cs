using Assignment.Dtos.Products;

namespace Assignment.Services
{
    public interface IProductService
    {
        Task<List<ProductListItemDto>> GetProductsAsync(string? search, Guid? categoryId, decimal? minPrice, decimal? maxPrice);
        Task<ProductDetailDto?> GetProductAsync(Guid id);

        Task<ProductDto> CreateAsync(ProductDto request);
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<ProductDto?> UpdateAsync(Guid id, ProductDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}

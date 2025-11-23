using Assignment.Dtos.Products;

namespace Assignment.Services
{
    public interface IComboService
    {
        Task<List<ComboListItemDto>> GetCombosAsync(string? search);
        Task<ComboDetailDto?> GetComboAsync(Guid id);
    }
}

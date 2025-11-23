using System.ComponentModel.DataAnnotations;

namespace Assignment.Dtos.Products
{
    public class ComboUpsertDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        [Range(0, 100, ErrorMessage = "Giảm giá combo phải trong khoảng 0-100%.")]
        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;
        public List<ComboUpsertItemDto> Items { get; set; } = new();
    }
}

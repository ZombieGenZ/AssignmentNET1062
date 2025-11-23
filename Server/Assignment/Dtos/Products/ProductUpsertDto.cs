using System.ComponentModel.DataAnnotations;

namespace Assignment.Dtos.Products
{
    public class ProductUpsertDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

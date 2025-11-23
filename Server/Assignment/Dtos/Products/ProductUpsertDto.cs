namespace Assignment.Dtos.Products
{
    public class ProductUpsertDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

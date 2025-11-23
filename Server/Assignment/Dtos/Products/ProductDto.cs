namespace Assignment.Dtos.Products
{
    public class ProductDto
    {
        public Guid? Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        public bool IsSpicy { get; set; }
        public bool IsVegetarian { get; set; }
        public decimal PriceMin { get; set; }
        public decimal PriceMax { get; set; }
        public int TotalStock { get; set; }
        public int TotalSold { get; set; }
        public Guid? PrimaryProductTypeId { get; set; }

        public List<ProductTypeDto> ProductTypes { get; set; } = new();
        public List<Guid> ExtraIds { get; set; } = new();
    }
}

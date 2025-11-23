namespace Assignment.Dtos.Products
{
    public class ProductListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
    }
}

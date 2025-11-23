namespace Assignment.Dtos.Products
{
    public class ProductDetailDto : ProductListItemDto
    {
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
    }
}

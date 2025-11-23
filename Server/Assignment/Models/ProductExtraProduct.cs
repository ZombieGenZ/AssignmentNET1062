namespace Assignment.Models
{
    public class ProductExtraProduct
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public Guid ProductExtraId { get; set; }
        public ProductExtra ProductExtra { get; set; } = null!;
    }
}

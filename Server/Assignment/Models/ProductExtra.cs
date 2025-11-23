namespace Assignment.Models
{
    public class ProductExtra
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ProductExtraProduct> ProductExtraProducts { get; set; } = new List<ProductExtraProduct>();
        public ICollection<ProductExtraCombo> ProductExtraCombos { get; set; } = new List<ProductExtraCombo>();
    }
}

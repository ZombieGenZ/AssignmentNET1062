namespace Assignment.Models
{
    public class Combo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public decimal DiscountPercent { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ComboItem> Items { get; set; } = new List<ComboItem>();
        public ICollection<ProductExtraCombo> ProductExtraCombos { get; set; } = new List<ProductExtraCombo>();

        public decimal CalculatePrice()
        {
            var total = Items.Sum(i => i.Product.Price * i.Quantity);
            var discount = total * (DiscountPercent / 100m);
            return total - discount;
        }
    }
}

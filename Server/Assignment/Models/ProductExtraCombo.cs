namespace Assignment.Models
{
    public class ProductExtraCombo
    {
        public Guid ComboId { get; set; }
        public Combo Combo { get; set; } = null!;

        public Guid ProductExtraId { get; set; }
        public ProductExtra ProductExtra { get; set; } = null!;
    }
}

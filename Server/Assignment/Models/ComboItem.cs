namespace Assignment.Models
{
    public class ComboItem
    {
        public Guid ComboId { get; set; }
        public Combo Combo { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
    }
}

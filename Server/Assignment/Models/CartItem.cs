using Assignment.Enums;

namespace Assignment.Models
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public CartItemType ItemType { get; set; }
        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid? ComboId { get; set; }
        public Combo? Combo { get; set; }

        public int Quantity { get; set; }
    }
}

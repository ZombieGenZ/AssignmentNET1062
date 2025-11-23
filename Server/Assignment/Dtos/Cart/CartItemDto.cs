using Assignment.Enums;

namespace Assignment.Dtos.Cart
{
    public class CartItemDto
    {
        public Guid Id { get; set; }
        public CartItemType ItemType { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ComboId { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}

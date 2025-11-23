using Assignment.Enums;

namespace Assignment.Dtos.Cart
{
    public class AddCartItemRequest
    {
        public CartItemType ItemType { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? ComboId { get; set; }
        public int Quantity { get; set; }
    }
}

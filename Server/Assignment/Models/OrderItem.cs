using Assignment.Enums;

namespace Assignment.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public OrderItemType ItemType { get; set; }
        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid? ComboId { get; set; }
        public Combo? Combo { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

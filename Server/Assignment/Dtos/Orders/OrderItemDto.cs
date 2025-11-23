using Assignment.Enums;

namespace Assignment.Dtos.Orders
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public OrderItemType ItemType { get; set; }
        public string Name { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}

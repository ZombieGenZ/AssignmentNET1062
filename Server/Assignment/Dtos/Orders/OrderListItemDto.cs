using Assignment.Enums;

namespace Assignment.Dtos.Orders
{
    public class OrderListItemDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
    }
}

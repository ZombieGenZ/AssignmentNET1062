using Assignment.Enums;

namespace Assignment.Dtos.Orders
{
    public class AdminOrderListItemDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus Status { get; set; }
    }
}

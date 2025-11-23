using Assignment.Enums;

namespace Assignment.Dtos.Orders
{
    public class OrderDetailDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string Address { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; }
        public PayNowGateway PayNowGateway { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public string? VoucherCode { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}

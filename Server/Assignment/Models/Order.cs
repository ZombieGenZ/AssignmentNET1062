using Assignment.Enums;

namespace Assignment.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public AppUser? User { get; set; }

        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string Address { get; set; } = null!;

        public PaymentMethod PaymentMethod { get; set; }
        public PayNowGateway PayNowGateway { get; set; } = PayNowGateway.None;

        public Guid? VoucherId { get; set; }
        public Voucher? Voucher { get; set; }

        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}

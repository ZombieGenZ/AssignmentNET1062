using Assignment.Enums;

namespace Assignment.Dtos.Orders
{
    public class CheckoutRequest
    {
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public string Address { get; set; } = null!;
        public PaymentMethod PaymentMethod { get; set; }
        public PayNowGateway? PayNowGateway { get; set; }
        public string? VoucherCode { get; set; }
        public List<Guid> CartItemIds { get; set; } = new();
    }
}

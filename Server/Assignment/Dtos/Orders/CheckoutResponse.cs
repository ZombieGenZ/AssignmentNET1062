namespace Assignment.Dtos.Orders
{
    public class CheckoutResponse
    {
        public Guid OrderId { get; set; }
        public string? PaymentUrl { get; set; }
    }
}

namespace Assignment.Dtos.Vouchers
{
    public class ValidateVoucherResponse
    {
        public bool IsValid { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Message { get; set; } = "";
    }
}

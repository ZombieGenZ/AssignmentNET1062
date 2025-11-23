namespace Assignment.Dtos.Vouchers
{
    public class ValidateVoucherRequest
    {
        public string Code { get; set; } = null!;
        public decimal OrderAmount { get; set; }
    }
}

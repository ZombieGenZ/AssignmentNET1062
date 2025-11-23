namespace Assignment.Models
{
    public class Voucher
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }

        public bool IsPublic { get; set; } = true;
        public decimal? DiscountPercent { get; set; }  // either %
        public decimal? DiscountAmount { get; set; }   // or fixed amount

        public decimal? MinOrderValue { get; set; }
        public int? MaxUsage { get; set; }
        public int UsedCount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

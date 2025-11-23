using System;
using System.ComponentModel.DataAnnotations;

namespace Assignment.Dtos.Vouchers
{
    public class VoucherUpsertDto
    {
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = true;
        [Range(0, 100, ErrorMessage = "Giảm giá phải trong khoảng 0-100%.")]
        public decimal? DiscountPercent { get; set; }
        public decimal? MinOrderValue { get; set; }
        public int? MaxUsage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

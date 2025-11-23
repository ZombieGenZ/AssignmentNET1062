using Assignment.Data;
using Assignment.Dtos.Vouchers;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly AppDbContext _db;

        public VoucherService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<VoucherDto>> GetPublicVouchersAsync()
        {
            var now = DateTime.UtcNow;

            return await _db.Vouchers
                .Where(v => v.IsPublic && v.IsActive && v.StartDate <= now && v.EndDate >= now)
                .Select(v => new VoucherDto
                {
                    Id = v.Id,
                    Code = v.Code,
                    Description = v.Description,
                    IsPublic = v.IsPublic,
                    DiscountPercent = v.DiscountPercent,
                    DiscountAmount = v.DiscountAmount,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    IsActive = v.IsActive
                }).ToListAsync();
        }

        public async Task<ValidateVoucherResponse> ValidateVoucherAsync(ValidateVoucherRequest request)
        {
            var now = DateTime.UtcNow;
            var voucher = await _db.Vouchers.FirstOrDefaultAsync(v => v.Code == request.Code);

            if (voucher == null || !voucher.IsActive || !voucher.IsPublic || voucher.StartDate > now || voucher.EndDate < now)
            {
                return new ValidateVoucherResponse
                {
                    IsValid = false,
                    DiscountAmount = 0,
                    Message = "Voucher không hợp lệ hoặc đã hết hạn."
                };
            }

            if (voucher.MinOrderValue.HasValue && request.OrderAmount < voucher.MinOrderValue.Value)
            {
                return new ValidateVoucherResponse
                {
                    IsValid = false,
                    DiscountAmount = 0,
                    Message = $"Đơn tối thiểu {voucher.MinOrderValue}."
                };
            }

            decimal discount = 0;
            if (voucher.DiscountPercent.HasValue)
                discount = request.OrderAmount * (voucher.DiscountPercent.Value / 100m);
            else if (voucher.DiscountAmount.HasValue)
                discount = voucher.DiscountAmount.Value;

            return new ValidateVoucherResponse
            {
                IsValid = true,
                DiscountAmount = discount,
                Message = "Áp dụng voucher thành công."
            };
        }
    }
}

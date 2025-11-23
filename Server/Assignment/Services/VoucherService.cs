using Assignment.Data;
using Assignment.Dtos.Vouchers;
using Assignment.Models;
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

        public async Task<List<VoucherAdminDto>> GetAllAsync()
        {
            var vouchers = await _db.Vouchers.OrderByDescending(v => v.StartDate).ToListAsync();
            return vouchers.Select(MapToAdminDto).ToList();
        }

        public async Task<VoucherAdminDto?> GetByIdAsync(Guid id)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            return voucher == null ? null : MapToAdminDto(voucher);
        }

        public async Task<VoucherAdminDto> CreateAsync(VoucherUpsertDto model)
        {
            if (await _db.Vouchers.AnyAsync(v => v.Code == model.Code))
            {
                throw new InvalidOperationException("Mã voucher đã tồn tại.");
            }

            var voucher = new Voucher
            {
                Id = Guid.NewGuid(),
                Code = model.Code,
                Description = model.Description,
                IsPublic = model.IsPublic,
                DiscountPercent = model.DiscountPercent,
                DiscountAmount = null,
                MinOrderValue = model.MinOrderValue,
                MaxUsage = model.MaxUsage,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive,
                UsedCount = 0
            };

            _db.Vouchers.Add(voucher);
            await _db.SaveChangesAsync();

            return MapToAdminDto(voucher);
        }

        public async Task<VoucherAdminDto?> UpdateAsync(Guid id, VoucherUpsertDto model)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null) return null;

            var existed = await _db.Vouchers.AnyAsync(v => v.Code == model.Code && v.Id != id);
            if (existed)
            {
                throw new InvalidOperationException("Mã voucher đã tồn tại.");
            }

            voucher.Code = model.Code;
            voucher.Description = model.Description;
            voucher.IsPublic = model.IsPublic;
            voucher.DiscountPercent = model.DiscountPercent;
            voucher.DiscountAmount = null;
            voucher.MinOrderValue = model.MinOrderValue;
            voucher.MaxUsage = model.MaxUsage;
            voucher.StartDate = model.StartDate;
            voucher.EndDate = model.EndDate;
            voucher.IsActive = model.IsActive;

            await _db.SaveChangesAsync();

            return MapToAdminDto(voucher);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null) return false;

            voucher.IsActive = false;
            await _db.SaveChangesAsync();
            return true;
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

            if (voucher.MaxUsage.HasValue && voucher.UsedCount >= voucher.MaxUsage.Value)
            {
                return new ValidateVoucherResponse
                {
                    IsValid = false,
                    DiscountAmount = 0,
                    Message = "Voucher đã hết lượt sử dụng."
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

            return new ValidateVoucherResponse
            {
                IsValid = true,
                DiscountAmount = discount,
                Message = "Áp dụng voucher thành công."
            };
        }

        private static VoucherAdminDto MapToAdminDto(Voucher v)
        {
            return new VoucherAdminDto
            {
                Id = v.Id,
                Code = v.Code,
                Description = v.Description,
                IsPublic = v.IsPublic,
                DiscountPercent = v.DiscountPercent,
                MinOrderValue = v.MinOrderValue,
                MaxUsage = v.MaxUsage,
                UsedCount = v.UsedCount,
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                IsActive = v.IsActive
            };
        }
    }
}

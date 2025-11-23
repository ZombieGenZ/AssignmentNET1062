using Assignment.Data;
using Assignment.Dtos.Vouchers;
using Assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/admin/vouchers")]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminVouchersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminVouchersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var vouchers = await _db.Vouchers.OrderByDescending(v => v.StartDate).ToListAsync();
            var result = vouchers.Select(MapToDto).ToList();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound();

            return Ok(MapToDto(voucher));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoucherUpsertDto model)
        {
            if (await _db.Vouchers.AnyAsync(v => v.Code == model.Code))
            {
                return Conflict(new { message = "Mã voucher đã tồn tại." });
            }

            var voucher = new Voucher
            {
                Id = Guid.NewGuid(),
                Code = model.Code,
                Description = model.Description,
                IsPublic = model.IsPublic,
                DiscountPercent = model.DiscountPercent,
                DiscountAmount = model.DiscountAmount,
                MinOrderValue = model.MinOrderValue,
                MaxUsage = model.MaxUsage,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive,
                UsedCount = 0
            };

            _db.Vouchers.Add(voucher);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = voucher.Id }, MapToDto(voucher));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VoucherUpsertDto model)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound();

            var existed = await _db.Vouchers.AnyAsync(v => v.Code == model.Code && v.Id != id);
            if (existed)
            {
                return Conflict(new { message = "Mã voucher đã tồn tại." });
            }

            voucher.Code = model.Code;
            voucher.Description = model.Description;
            voucher.IsPublic = model.IsPublic;
            voucher.DiscountPercent = model.DiscountPercent;
            voucher.DiscountAmount = model.DiscountAmount;
            voucher.MinOrderValue = model.MinOrderValue;
            voucher.MaxUsage = model.MaxUsage;
            voucher.StartDate = model.StartDate;
            voucher.EndDate = model.EndDate;
            voucher.IsActive = model.IsActive;

            await _db.SaveChangesAsync();

            return Ok(MapToDto(voucher));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var voucher = await _db.Vouchers.FindAsync(id);
            if (voucher == null) return NotFound();

            voucher.IsActive = false;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private static VoucherAdminDto MapToDto(Voucher v)
        {
            return new VoucherAdminDto
            {
                Id = v.Id,
                Code = v.Code,
                Description = v.Description,
                IsPublic = v.IsPublic,
                DiscountPercent = v.DiscountPercent,
                DiscountAmount = v.DiscountAmount,
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

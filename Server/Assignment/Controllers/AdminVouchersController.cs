using Assignment.Dtos.Vouchers;
using Assignment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/admin/vouchers")]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminVouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public AdminVouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var vouchers = await _voucherService.GetAllAsync();
            return Ok(vouchers);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null) return NotFound();

            return Ok(voucher);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoucherUpsertDto model)
        {
            try
            {
                var voucher = await _voucherService.CreateAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = voucher.Id }, voucher);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VoucherUpsertDto model)
        {
            try
            {
                var voucher = await _voucherService.UpdateAsync(id, model);
                if (voucher == null) return NotFound();

                return Ok(voucher);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _voucherService.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}

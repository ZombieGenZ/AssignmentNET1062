using Assignment.Dtos.Vouchers;
using Assignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        public VouchersController(IVoucherService voucherService) { _voucherService = voucherService; }

        [HttpGet("public")]
        public async Task<ActionResult<List<VoucherDto>>> GetPublic()
        {
            var list = await _voucherService.GetPublicVouchersAsync();
            return Ok(list);
        }
    }
}

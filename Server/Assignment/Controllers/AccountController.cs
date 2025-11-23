using Assignment.Dtos.Account;
using Assignment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ProfileResponse>> GetProfile()
        {
            var profile = await _accountService.GetProfileAsync(User);
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<ProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var profile = await _accountService.UpdateProfileAsync(User, request);
            return Ok(profile);
        }

        [HttpGet("login-methods")]
        public async Task<ActionResult<LoginMethodsResponse>> GetLoginMethods()
        {
            var result = await _accountService.GetLoginMethodsAsync(User);
            return Ok(result);
        }

        [HttpDelete("unlink-login")]
        public async Task<IActionResult> UnlinkLogin([FromBody] UnlinkLoginRequest request)
        {
            await _accountService.UnlinkLoginAsync(User, request.Provider);
            return NoContent();
        }

        [HttpPost("link-google")]
        public IActionResult LinkGoogle()
        {
            return BadRequest("Chưa implement link Google.");
        }

        [HttpPost("link-facebook")]
        public IActionResult LinkFacebook()
        {
            return BadRequest("Chưa implement link Facebook.");
        }
    }
}

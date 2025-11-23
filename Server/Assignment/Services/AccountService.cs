using Assignment.Dtos.Account;
using Assignment.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Assignment.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        private async Task<AppUser> GetCurrentUser(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null) throw new ApplicationException("User không tồn tại.");
            return user;
        }

        public async Task<ProfileResponse> GetProfileAsync(ClaimsPrincipal principal)
        {
            var user = await GetCurrentUser(principal);
            var roles = await _userManager.GetRolesAsync(user);

            return new ProfileResponse
            {
                Id = user.Id,
                FullName = user.FullName ?? "",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Gender = user.Gender,
                Roles = roles.ToList()
            };
        }

        public async Task<ProfileResponse> UpdateProfileAsync(ClaimsPrincipal principal, UpdateProfileRequest request)
        {
            var user = await GetCurrentUser(principal);

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.Gender = request.Gender;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ApplicationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            var roles = await _userManager.GetRolesAsync(user);

            return new ProfileResponse
            {
                Id = user.Id,
                FullName = user.FullName ?? "",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Gender = user.Gender,
                Roles = roles.ToList()
            };
        }

        public async Task<LoginMethodsResponse> GetLoginMethodsAsync(ClaimsPrincipal principal)
        {
            var user = await GetCurrentUser(principal);
            var logins = await _userManager.GetLoginsAsync(user);
            var methods = new List<LoginMethodDto>();

            // Password được xem là provider "Password"
            methods.Add(new LoginMethodDto { Provider = "Password", IsPrimary = true });
            foreach (var login in logins)
            {
                methods.Add(new LoginMethodDto
                {
                    Provider = login.LoginProvider,
                    IsPrimary = false // demo: Password là gốc
                });
            }

            return new LoginMethodsResponse
            {
                Primary = "Password",
                Logins = methods
            };
        }

        public async Task UnlinkLoginAsync(ClaimsPrincipal principal, string provider)
        {
            if (provider == "Password")
                throw new ApplicationException("Không thể gỡ phương thức đăng nhập gốc.");

            var user = await GetCurrentUser(principal);
            var logins = await _userManager.GetLoginsAsync(user);

            var login = logins.FirstOrDefault(l => l.LoginProvider == provider);
            if (login == null)
                throw new ApplicationException("Không tìm thấy phương thức đăng nhập.");

            var result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            if (!result.Succeeded)
                throw new ApplicationException("Không thể gỡ liên kết.");
        }
    }
}

using Assignment.Data;
using Assignment.Dtos.Auth;
using Assignment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            AppDbContext db,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email))
                throw new ApplicationException("Email đã tồn tại.");

            if (await _userManager.Users.AnyAsync(x => x.PhoneNumber == request.PhoneNumber))
                throw new ApplicationException("Số điện thoại đã tồn tại.");

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                Address = request.Address,
                Gender = request.Gender
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new ApplicationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new AppRole { Name = "User" });

            await _userManager.AddToRoleAsync(user, "User");

            var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user);
            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == request.Identifier || u.PhoneNumber == request.Identifier);

            if (user == null)
                throw new ApplicationException("Sai thông tin đăng nhập.");

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new ApplicationException("Sai thông tin đăng nhập.");

            var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(user);
            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };
        }

        public async Task<AuthResponse> RefreshAsync(string refreshToken)
        {
            var tokenEntity = await _db.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiresAt < DateTime.UtcNow)
                throw new ApplicationException("Refresh token không hợp lệ.");

            tokenEntity.IsRevoked = true;

            var (accessToken, newRefreshToken) = await _tokenService.GenerateTokensAsync(tokenEntity.User);
            _db.RefreshTokens.Add(newRefreshToken);

            await _db.SaveChangesAsync();

            return new AuthResponse { AccessToken = accessToken, RefreshToken = newRefreshToken.Token };
        }
    }
}

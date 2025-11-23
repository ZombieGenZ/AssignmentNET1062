using Assignment.Models;

namespace Assignment.Services
{
    public interface ITokenService
    {
        Task<(string accessToken, RefreshToken refreshToken)> GenerateTokensAsync(AppUser user);
        string GenerateAccessToken(AppUser user, IList<string> roles);
        RefreshToken GenerateRefreshToken(Guid userId);
    }
}

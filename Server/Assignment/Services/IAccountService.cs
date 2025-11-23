using Assignment.Dtos.Account;
using System.Security.Claims;

namespace Assignment.Services
{
    public interface IAccountService
    {
        Task<ProfileResponse> GetProfileAsync(ClaimsPrincipal user);
        Task<ProfileResponse> UpdateProfileAsync(ClaimsPrincipal principal, UpdateProfileRequest request);
        Task<LoginMethodsResponse> GetLoginMethodsAsync(ClaimsPrincipal principal);
        Task UnlinkLoginAsync(ClaimsPrincipal principal, string provider);
    }
}

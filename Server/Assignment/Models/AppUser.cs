using Microsoft.AspNetCore.Identity;

namespace Assignment.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<AppUserLogin> ExternalLogins { get; set; } = new List<AppUserLogin>();
    }
}

using Microsoft.AspNetCore.Identity;

namespace Assignment.Models
{
    public class AppUserLogin : IdentityUserLogin<Guid>
    {
        public bool IsPrimary { get; set; }
    }
}

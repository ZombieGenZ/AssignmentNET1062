using Microsoft.AspNetCore.Identity;

namespace Assignment.Models
{
    public class AppRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
    }
}

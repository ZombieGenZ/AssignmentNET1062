namespace Assignment.Dtos.Account
{
    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
    }
}

namespace Assignment.Dtos.Account
{
    public class ProfileResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}

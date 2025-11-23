namespace Assignment.Dtos.Auth
{
    public class RegisterRequest
    {
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Gender { get; set; } = "other";
        public string Password { get; set; } = null!;
    }
}

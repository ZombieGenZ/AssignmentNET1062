namespace Assignment.Dtos.Account
{
    public class LoginMethodDto
    {
        public string Provider { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}

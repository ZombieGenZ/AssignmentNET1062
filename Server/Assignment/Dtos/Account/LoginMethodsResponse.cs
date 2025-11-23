namespace Assignment.Dtos.Account
{
    public class LoginMethodsResponse
    {
        public string Primary { get; set; } = null!;
        public List<LoginMethodDto> Logins { get; set; } = new();
    }
}

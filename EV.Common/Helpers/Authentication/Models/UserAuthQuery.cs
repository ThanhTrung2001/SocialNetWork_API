namespace EV.Common.Helpers.Authentication.Models
{
    public class UserAuthQuery
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string? Role { get; set; }
    }
}

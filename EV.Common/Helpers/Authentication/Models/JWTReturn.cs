namespace EV.Common.Helpers.Authentication.Models
{
    public class JWTReturn
    {
        public string? Token { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpiredIn { get; set; }
    }
}

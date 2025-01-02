namespace EnVietSocialNetWorkAPI.Auth.Services
{
    public class JWTReturn
    {
        public string Token { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpiredIn { get; set; }
    }
}

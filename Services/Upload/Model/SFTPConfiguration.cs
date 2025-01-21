namespace EnVietSocialNetWorkAPI.Services.Upload.Model
{
    public class SFTPConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }
        public string DisplayUrl { get; set; }
    }
}

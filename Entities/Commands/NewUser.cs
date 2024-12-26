using EnVietSocialNetWorkAPI.Entities.Models;

namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class NewUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public UserRole Role { get; set; }
    }
}

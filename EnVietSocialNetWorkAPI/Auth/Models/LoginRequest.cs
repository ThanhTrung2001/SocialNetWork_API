using System.ComponentModel.DataAnnotations;

namespace EnVietSocialNetWorkAPI.Auth.Model
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace EnVietSocialNetWorkAPI.Auth.Models
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string RetypePassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
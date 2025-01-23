using System.ComponentModel.DataAnnotations;

namespace EV.Common.Helpers.Authentication.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}

using EV.Common.Helpers.Authentication.Models;
using EV.Common.SettingConfigurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EV.Common.Helpers.Authentication
{
    public class JWTHelper
    {
        private readonly JWTConfiguration _config;

        public JWTHelper(IOptions<JWTConfiguration> configuration)
        {
            _config = configuration.Value;
        }

        public JWTReturn GenerateJWTToken(UserAuthQuery user)
        {
            var claims = new List<Claim> {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName!),
                        new Claim(ClaimTypes.Email, user.Email!),
                        new Claim("AvatarUrl", user.Avatar ?? string.Empty),
                        new Claim("Role", user.Role ?? string.Empty) };
            //
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key!));
            //
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _config.Issuer!,
                Audience = _config.Audience!,
                Expires = DateTime.UtcNow.AddDays(200),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtReturn = new JWTReturn();
            jwtReturn.CreatedAt = DateTime.UtcNow;
            jwtReturn.ExpiredIn = tokenDescriptor.Expires;
            jwtReturn.Token = tokenHandler.WriteToken(token);
            return jwtReturn;
        }

        public static bool IsTokenExpired(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtTokenObj = tokenHandler.ReadJwtToken(token);
            // Retrieve the token's expiration time
            var expires = jwtTokenObj.ValidTo;
            Debug.WriteLine(expires);
            // Check if the token has expired
            var isExpired = DateTime.UtcNow > expires;
            return isExpired;
        }
    }
}

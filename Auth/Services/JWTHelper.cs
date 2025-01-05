using EnVietSocialNetWorkAPI.Models.Queries;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EnVietSocialNetWorkAPI.Auth.Services
{
    public class JWTHelper
    {
        private readonly IConfiguration _configuration;

        public JWTHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JWTReturn GenerateJWTToken(UserAuthQuery user)
        {
            var claims = new List<Claim> {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("AvatarUrl", user.Avatar ?? string.Empty) };
            //
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            //
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Expires = DateTime.UtcNow.AddDays(100),
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

        //_helper func for tree hierarchy

        public List<OrganizeNodeQuery> BuildHierarchy(List<OrganizeNodeQuery> nodes)
        {
            // Find root nodes
            var rootNodes = nodes.Where(n => n.ParentId == null || n.ParentId == Guid.Empty).ToList();

            foreach (var rootNode in rootNodes)
            {
                AttachChildren(rootNode, nodes);
            }

            return rootNodes;
        }

        public void AttachChildren(OrganizeNodeQuery parentNode, List<OrganizeNodeQuery> allNodes)
        {
            var children = allNodes.Where(n => n.ParentId == parentNode.Id).ToList();
            parentNode.ChildrenNodes = children;

            foreach (var child in children)
            {
                AttachChildren(child, allNodes);
            }
        }
    }
}

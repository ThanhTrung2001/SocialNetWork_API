using Dapper;
using EnVietSocialNetWorkAPI.Auth.Model;
using EnVietSocialNetWorkAPI.Auth.Services;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly JWTHelper _helper;

        public SessionController(DapperContext context, JWTHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var query = "SELECT * FROM Users WHERE Email Like @Email AND Password Like @Password;";
            var parameter = new DynamicParameters();
            parameter.Add("Email", request.Email);
            parameter.Add("Password", request.Password);
            var token = "";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<UserQuery>(query, parameter);
                if (result != null)
                {
                    token = _helper.GenerateJWTToken(result);
                }
            }
            return Ok(token);

        }
    }
}

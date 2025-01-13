using Dapper;
using EnVietSocialNetWorkAPI.Auth.Model;
using EnVietSocialNetWorkAPI.Auth.Services;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Queries;
using EnVietSocialNetWorkAPI.Models;
using EnVietSocialNetWorkAPI.Services.Email;
using EnVietSocialNetWorkAPI.Services.Email.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly JWTHelper _helper;
        private readonly IEmailHandler _handler;

        public SessionController(DapperContext context, JWTHelper helper, IEmailHandler handler)
        {
            _context = context;
            _helper = helper;
            _handler = handler;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var query = @"SELECT 
                          u.Id, u.UserName, u.Email, u.Password, u.Role, ud.Avatar 
                          FROM Users u
                          INNER JOIN User_Details ud ON u.Id = ud.User_Id  
                          WHERE u.Email Like @Email AND u.Password Like @Password;";
            var parameter = new DynamicParameters();
            parameter.Add("Email", request.Email);
            parameter.Add("Password", request.Password);
            var token = new JWTReturn();
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync<UserAuthQuery>(query, parameter);
                    if (result != null)
                    {
                        token = _helper.GenerateJWTToken(result);
                    }
                    return Ok(ResponseModel<JWTReturn>.Success(token));
                }

                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<JWTReturn>.Failure(ex.Message));
                }

            }
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(EmailMessage message)
        {
            try
            {
                _handler.SendEmailAsync(message);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models;
using EnVietSocialNetWorkAPI.Entities.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DapperContext _context;
        public UsersController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<UserQuery>> Get()
        {
            var query = "SELECT Id, UserName, AvatarUrl, Email FROM Users";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<UserQuery>(query);
                return result;
            }
        }

        [HttpGet("/search")]
        public async Task<IEnumerable<User>> GetBySearch([FromQuery] string name)
        {
            var query = @"SELECT Id, UserName, AvatarUrl, Email FROM Users WHERE UserName LIKE @name;";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<User>(query, new { name = $"%{name}%" });
                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<User> GetByID(Guid id)
        {
            var query = "SELECT Id, UserName, AvatarUrl, Email FROM Users where Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
                return result;
            }
        }

        [HttpPost]
        public async Task<Guid> AddNewUser(NewUser user)
        {
            var exec = @"INSERT INTO Users (Id, CreatedAt, UpdatedAt, IsDeleted, UserName, Password, Email, AvatarUrl, Role)
                        OUTPUT Inserted.Id
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @UserName, @Password, @Email, @AvatarUrl, @Role);";
            var parameters = new DynamicParameters();
            parameters.Add("UserName", user.UserName, DbType.String);
            parameters.Add("Password", user.Password, DbType.String);
            parameters.Add("Email", user.Email, DbType.String);
            parameters.Add("AvatarUrl", user.AvatarUrl, DbType.String);
            parameters.Add("Role", user.Role, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleAsync<Guid>(exec, parameters);
                return result;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditUserByID(Guid id, NewUser edit)
        {
            var exec = "UPDATE Users SET UserName = @UserName, Password = @Password, Email = @Email, AvatarUrl = @AvatarUrl, Role = @Role WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("UserName", edit.UserName, DbType.String);
            parameters.Add("Password", edit.Password, DbType.String);
            parameters.Add("Email", edit.Email, DbType.String);
            parameters.Add("AvatarUrl", edit.AvatarUrl, DbType.String);
            parameters.Add("Role", edit.Role, DbType.Int32);
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(exec, parameters);
                return Ok(result);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var exec = "Update Users SET isDeleted = 1 WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(exec, new { Id = id });
                return Ok();
            }
        }


    }
}

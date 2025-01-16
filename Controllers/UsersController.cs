using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
using EnVietSocialNetWorkAPI.Models;
using EnVietSocialNetWorkAPI.Services.Email;
using EnVietSocialNetWorkAPI.Services.Email.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly IEmailHandler _handler;
        public UsersController(DapperContext context, IEmailHandler handler)
        {
            _context = context;
            _handler = handler;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var query = @"SELECT 
                            u.Id,
                            u.Role,
                            ud.FirstName,
                            ud.LastName,
                            ud.Avatar
                          FROM Users u
                          INNER JOIN User_Details ud ON u.Id = ud.User_Id
                          WHERE u.Is_Deleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<UserQuery>(query);
                    return Ok(ResponseModel<IEnumerable<UserQuery>>.Success(result));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<UserQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetBySearch([FromQuery] string name)
        {
            var query = @"SELECT 
                            u.Id,
                            u.Role,
                            ud.FirstName,
                            ud.LastName,
                            ud.Avatar
                          FROM Users u
                          INNER JOIN User_Details ud ON u.Id = ud.User_Id
                          WHERE u.Is_Deleted = 0 AND (ud.FirstName Like @Name OR ud.LastName Like @Name);";
            var parameter = new DynamicParameters();
            parameter.Add("Name", $"%{name}%");
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<UserQuery>(query, parameter);
                    return Ok(ResponseModel<IEnumerable<UserQuery>>.Success(result));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<UserQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var query = @"SELECT 
                            u.Id,
                            u.UserName,
                            u.Email,
                            u.Role,
                            ud.FirstName,
                            ud.LastName,
                            ud.Phone_Number,
                            ud.Address,
                            ud.City,
                            ud.Country,
                            ud.Avatar,
                            ud.Wallpaper,
                            ud.DOB,
                            ud.Bio
                          FROM Users u
                          INNER JOIN User_Details ud ON u.Id = ud.User_Id
                          WHERE u.Is_Deleted = 0 AND u.Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {

                    var result = await connection.QuerySingleAsync<UserQueryDetail>(query, parameter);
                    return Ok(ResponseModel<UserQueryDetail>.Success(result));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<UserQueryDetail>.Failure(ex.Message));

                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserCommand user)
        {
            var existEmailQuery = @"SELECT u.Id,
                                    u.Role,
                                    ud.FirstName,
                                    ud.LastName,
                                    ud.Avatar
                                  FROM Users u
                                  INNER JOIN User_Details ud ON u.Id = ud.User_Id
                                  WHERE u.Email LIKE @Email";
            var queryUser = @"INSERT INTO Users (Id, Created_At, Updated_At, Is_Deleted, UserName, Password, Email, Role)
                        OUTPUT Inserted.Id
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @UserName, @Password, @Email, @Role);";
            var queryUserDetail = @"INSERT INTO User_Details (Id, Created_At, Updated_At, Is_Deleted, FirstName, LastName, Phone_Number, Address, City, Country, Avatar, Wallpaper, DOB, Bio, User_Id)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @FirstName, @LastName, @Phone_Number, @Address, @City, @Country, @Avatar, @Wallpaper, @DOB, @Bio, @User_Id);";
            var parameters = new DynamicParameters();
            parameters.Add("UserName", user.UserName, DbType.String);
            parameters.Add("Password", user.Password, DbType.String);
            parameters.Add("Email", user.Email, DbType.String);
            parameters.Add("Role", user.Role, DbType.String);
            parameters.Add("FirstName", user.FirstName, DbType.String);
            parameters.Add("LastName", user.LastName, DbType.String);
            parameters.Add("Phone_Number", user.Phone_Number);
            parameters.Add("Address", user.Address, DbType.String);
            parameters.Add("City", user.City, DbType.String);
            parameters.Add("Country", user.Country, DbType.String);
            parameters.Add("Avatar", user.Avatar, DbType.String);
            parameters.Add("Wallpaper", user.Wallpaper, DbType.String);
            parameters.Add("DOB", user.DOB, DbType.DateTime);
            parameters.Add("Bio", user.Bio, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var existResult = await connection.QueryAsync<UserQuery>(existEmailQuery, parameters, transaction);
                        if (existResult.Count() >= 1)
                        {
                            return BadRequest(ResponseModel<Guid>.Failure("User with this Email is existed!"));
                        }
                        var result = await connection.QuerySingleAsync<Guid>(queryUser, parameters, transaction);
                        parameters.Add("User_Id", result);
                        await connection.ExecuteAsync(queryUserDetail, parameters, transaction);
                        transaction.Commit();
                        _handler.SendEmail(new EmailMessage() { ToEmails = [user.Email], Subject = "Registered Successful.", Body = "<h1>Welcome to our app<h1>" });
                        return Ok(ResponseModel<Guid>.Success(result));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<Guid>.Failure(ex.Message));
                    }
                }

            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, EditUserCommand edit)
        {
            var query = "UPDATE Users SET Updated_At = GETDATE(), UserName = @UserName, Email = @Email, Role = @Role WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("UserName", edit.UserName, DbType.String);
            parameters.Add("Email", edit.Email, DbType.String);
            parameters.Add("Role", edit.Role);
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {

                    await connection.ExecuteAsync(query, parameters);
                    return Ok(ResponseModel<string>.Success("Update Successful."));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpPut("{id}/detail")]
        public async Task<IActionResult> EditUserDetail(Guid id, EditUserDetailCommand edit)
        {
            var query = "UPDATE User_Details SET Updated_At = GETDATE(), FirstName = @FirstName, LastName = @LastName, Phone_Number=@Phone_Number, Address = @Address, City = @City, Country = @Country ,Avatar = @Avatar, Wallpaper = @Wallpaper, DOB = @DOB, Bio = @Bio WHERE User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("FirstName", edit.FirstName, DbType.String);
            parameters.Add("LastName", edit.LastName, DbType.String);
            parameters.Add("Phone_Number", edit.Phone_Number);
            parameters.Add("Address", edit.Address);
            parameters.Add("City", edit.City);
            parameters.Add("Country", edit.Country);
            parameters.Add("Avatar", edit.Avatar, DbType.String);
            parameters.Add("Wallpaper", edit.Wallpaper, DbType.String);
            parameters.Add("DOB", edit.DOB, DbType.String);
            parameters.Add("Bio", edit.Bio, DbType.String);
            parameters.Add("User_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {

                    var result = await connection.ExecuteAsync(query, parameters);
                    return Ok(ResponseModel<string>.Success("Update Successful."));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpPut("{id}/password")]
        public async Task<IActionResult> ChangePassword(Guid id, string password)
        {
            var queryUser = "Update Users SET Updated_At = GETDATE(), Password = @Password WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            parameter.Add("Password", password);
            using (var connection = _context.CreateConnection())
            {
                try
                {

                    await connection.ExecuteAsync(queryUser, parameter);
                    return Ok(ResponseModel<string>.Success("Change Password Successful."));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var queryUser = "Update Users SET Updated_At = GETDATE(), Is_Deleted = 1 WHERE Id = @Id";
            var queryUserDetail = "Update User_Details SET Updated_At = GETDATE(), Is_Deleted = 1 WHERE User_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(queryUser, parameter);
                await connection.ExecuteAsync(queryUserDetail, parameter);
                try
                {

                    return Ok(ResponseModel<string>.Success("Delete Successful."));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }


    }
}

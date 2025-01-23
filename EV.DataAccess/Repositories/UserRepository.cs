using Dapper;
using EV.Common.Services.Email;
using EV.Common.Services.Email.Model;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;
using System.Data;

namespace EV.DataAccess.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseContext _context;
        private readonly IEmailHandler _handler;

        public UserRepository(DatabaseContext context, IEmailHandler handler)
        {
            _context = context;
            _handler = handler;
        }

        public async Task<ResponseModel<IEnumerable<UserQuery>>> GetAll()
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
                    return ResponseModel<IEnumerable<UserQuery>>.Success(result);
                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<UserQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<IEnumerable<UserQuery>>> Search(string name)
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
                    return ResponseModel<IEnumerable<UserQuery>>.Success(result);
                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<UserQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<UserQueryDetail>> GetByID(Guid id)
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
                    return ResponseModel<UserQueryDetail>.Success(result);
                }
                catch (Exception ex)
                {
                    return ResponseModel<UserQueryDetail>.Failure(ex.Message)!;

                }
            }
        }

        public async Task<ResponseModel<Guid>> Create(CreateUserCommand user)
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
                            return ResponseModel<Guid>.Failure("User with this Email is existed!");
                        }
                        var result = await connection.QuerySingleAsync<Guid>(queryUser, parameters, transaction);
                        parameters.Add("User_Id", result);
                        await connection.ExecuteAsync(queryUserDetail, parameters, transaction);
                        transaction.Commit();
                        _handler.SendEmail(new EmailMessage() { Receivers = [user.Email], Subject = "Registered Successful.", Body = "<h1>Welcome to our app<h1>" });
                        return ResponseModel<Guid>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ResponseModel<Guid>.Failure(ex.Message);
                    }
                }

            }
        }

        public async Task<ResponseModel<string>> Edit(Guid id, EditUserCommand command)
        {
            var query = "UPDATE Users SET Updated_At = GETDATE(), UserName = @UserName, Email = @Email, Role = @Role WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("UserName", command.UserName, DbType.String);
            parameters.Add("Email", command.Email, DbType.String);
            parameters.Add("Role", command.Role);
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {

                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Delete Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message);
                }
            }
        }

        public async Task<ResponseModel<string>> EditDetail(Guid user_Id, EditUserDetailCommand command)
        {
            var query = "UPDATE User_Details SET Updated_At = GETDATE(), FirstName = @FirstName, LastName = @LastName, Phone_Number=@Phone_Number, Address = @Address, City = @City, Country = @Country ,Avatar = @Avatar, Wallpaper = @Wallpaper, DOB = @DOB, Bio = @Bio WHERE User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("FirstName", command.FirstName, DbType.String);
            parameters.Add("LastName", command.LastName, DbType.String);
            parameters.Add("Phone_Number", command.Phone_Number);
            parameters.Add("Address", command.Address);
            parameters.Add("City", command.City);
            parameters.Add("Country", command.Country);
            parameters.Add("Avatar", command.Avatar, DbType.String);
            parameters.Add("Wallpaper", command.Wallpaper, DbType.String);
            parameters.Add("DOB", command.DOB, DbType.DateTime);
            parameters.Add("Bio", command.Bio, DbType.String);
            parameters.Add("User_Id", user_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {

                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Delete Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> ChangePassword(Guid id, ChangeUserPasswordCommand command)
        {
            var queryUser = "Update Users SET Updated_At = GETDATE(), Password = @Password WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            parameter.Add("Password", command.Password);
            using (var connection = _context.CreateConnection())
            {
                try
                {

                    var rowEffect = await connection.ExecuteAsync(queryUser, parameter);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("ChangePassword Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Delete(Guid id)
        {
            var queryUser = "Update Users SET Updated_At = GETDATE(), Is_Deleted = 1 WHERE Id = @Id";
            var queryUserDetail = "Update User_Details SET Updated_At = GETDATE(), Is_Deleted = 1 WHERE User_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowUserEffect = await connection.ExecuteAsync(queryUser, parameter);
                    var rowUserDetailEffect = await connection.ExecuteAsync(queryUserDetail, parameter);
                    return (rowUserEffect > 0 & rowUserDetailEffect > 0) ? ResponseModel<string>.Success("Delete Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }
    }
}

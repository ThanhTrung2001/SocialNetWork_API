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

        [HttpGet("{id}/posts")]
        public async Task<IEnumerable<PostBasicQuery>> GetUserPostByUserID(Guid id)
        {
            var query = @"
            SELECT 
                p.Id AS PostId,
                p.Content AS PostContent,
                p.PostType,
                p.CreatedAt,
                p.PostDestination,
                u.Id AS UserId,
                u.UserName,
                u.Email,
                u.AvatarUrl,
                m.URL AS MediaUrl,
                c.Id AS CommentId,
                c.Content AS CommentContent,
                c.MediaURL AS CommentMediaUrl,
                c.CreatedAt AS CommentCreatedAt,
                c.UserId AS CommentUserId,
                uc.UserName AS CommentUserName,
                uc.AvatarUrl AS CommentUserAvatarUrl,
                r.Id AS ReactId,
                r.ReactType,
                ur.Id AS ReactUserId,
                ur.UserName AS ReactUserName,
                ur.AvatarUrl AS ReactUserAvatar
            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.OwnerId = u.Id
            LEFT JOIN
                MediaItems m ON p.Id = m.PostId
            LEFT JOIN
                Comments c ON p.Id = c.PostId
            LEFT JOIN
                Users uc ON c.UserId = uc.Id
            LEFT JOIN 
                Reacts r ON p.Id = r.PostId
            LEFT JOIN
                Users ur ON r.UserId = ur.Id 
            WHERE 
                p.IsDeleted = 0 AND p.OwnerId = @Id AND p.PostType = 'personal';";

            try
            {
                var postDict = new Dictionary<Guid, PostBasicQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostBasicQuery, string, PostCommentQuery, PostReactQuery, PostBasicQuery>(
                    query,
                    map: (post, mediaUrl, comment, react) =>
                    {
                        if (!postDict.TryGetValue(post.PostId, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.PostId, postEntry);
                        }

                        if (!string.IsNullOrEmpty(mediaUrl) && !postEntry.MediaUrls.Any((item) => item == mediaUrl))
                        {
                            postEntry.MediaUrls.Add(mediaUrl);
                        }

                        if (comment != null && !postEntry.Comments.Any((item) => item.CommentId == comment.CommentId))
                        {
                            postEntry.Comments.Add(comment);
                        }
                        if (react != null && !postEntry.Reacts.Any((item) => item.ReactId == react.ReactId))
                        {
                            postEntry.Reacts.Add(react);
                        }
                        return postEntry;
                    },
                    new { Id = id },
                    splitOn: "MediaUrl,CommentId, ReactId");
                    return postDict.Values.ToList();
                }
            }
            catch
            {
                throw;
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

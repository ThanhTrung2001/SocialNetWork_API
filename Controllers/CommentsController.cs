using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly DapperContext _context;
        public CommentsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("post/{postId}")]
        public async Task<IEnumerable<CommentQuery>> GetCommentsByPostID(Guid postId)
        {
            var query = @"SELECT c.Id, c.Content, c.MediaURL, c.UpdatedAt, u.Id as UserId, u.Username, u.AvatarUrl, c.ReactCount
                          FROM Comments c
                          INNER JOIN Users u ON c.UserId = u.Id 
                          Where c.PostId = @Id & c.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", postId);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<CommentQuery>(query, parameter);

                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<CommentDetailQuery> GetCommentByID(Guid id)
        {
            var query = @"SELECT c.Id, c.Content, c.MediaURL, c.UpdatedAt, u.Id as UserId, u.Username, u.AvatarUrl, cr.Id AS ReactId, cr.ReactType, cr.UserId AS ReactUserId, ucr.UserName AS ReactUserName, ucr.AvatarUrl AS ReactUserAvatar 
                          FROM Comments c
                          INNER JOIN Users u ON c.UserId = u.Id 
                          LEFT JOIN CommentReacts cr ON c.Id = cr.CommentId
                          LEFT JOIN Users ucr ON cr.UserId = ucr.Id
                          Where c.Id = @Id & c.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var commentResult = new CommentDetailQuery();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<CommentDetailQuery, CommentReactQuery, CommentDetailQuery>(
                    query,
                    map: (comment, react) =>
                    {
                        if (comment != null)
                        {
                            commentResult = comment;
                        }

                        if (react != null && !commentResult.Reacts.Any((item) => item.ReactId == react.ReactId))
                        {
                            commentResult.Reacts.Add(react);
                        }
                        return commentResult;
                    },
                    parameter,
                    splitOn: "ReactId");
                    return commentResult;
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("post/{postId}")]
        public async Task<IActionResult> CreateComment(Guid postId, NewComment comment)
        {
            var query = @"INSERT INTO Comments (Id, CreatedAt, UpdatedAt, IsDeleted, Content, MediaURL, UserId, PostId)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Content, @MediaURL, @UserId, @PostId)";
            var parameters = new DynamicParameters();
            parameters.Add("Content", comment.Content, DbType.String);
            parameters.Add("MediaURL", comment.MediaURL, DbType.String);
            parameters.Add("UserId", comment.UserId, DbType.Guid);
            parameters.Add("PostId", postId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditComment(Guid id, NewComment comment)
        {
            var query = "UPDATE Comments SET Content = @Content, MediaURL = @MediaURL WHERE Id = @Id AND UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("Content", comment.Content, DbType.String);
            parameters.Add("MediaURL", comment.MediaURL, DbType.String);
            parameters.Add("UserId", comment.UserId, DbType.Guid);
            parameters.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update Comments SET isDeleted = 1 WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }
    }
}

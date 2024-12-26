using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Route("api/[controller]/post")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly DapperContext _context;
        public CommentsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("{postId}")]
        public async Task<IEnumerable<Comment>> GetCommentByPostID(Guid postId)
        {
            var query = "SELECT * FROM Comments Where PostId = @Id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Comment>(query, new { Id = postId });
                return result;
            }
        }

        [HttpPost("{postId}")]
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

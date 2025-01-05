using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentReactsController : ControllerBase
    {
        private readonly DapperContext _context;

        public CommentReactsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("comment/{commentId}")]
        public async Task<IEnumerable<CommentReactQuery>> GetByCommentId(Guid commentId)
        {
            var query = @"SELECT c.Id AS ReactId, c.ReactType AS Type, c.UserId AS ReactUserId, u.UserName AS ReactUserName, u.AvatarUrl AS ReactUserAvatar
                          FROM CommentReacts c
                          LEFT JOIN User u ON u.Id = c.UserId AND c.IsDeleted = 0 AND c.CommentId = @CommentId";
            var parameter = new DynamicParameters();
            parameter.Add("CommentId", commentId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<CommentReactQuery>(query, parameter);
                return result;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateByCommentId(NewCommentReact react)
        {
            var query = @"INSERT INTO CommentReacts (Id, CreatedAt, UpdatedAt, IsDeleted, ReactType, UserId, PostId)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @ReactType, @UserId, @CommentId)";
            var parameters = new DynamicParameters();
            //parameters.Add("ReactType", react.ReactType, DbType.Int32);
            parameters.Add("UserId", react.UserId, DbType.Guid);
            parameters.Add("CommentId", react.CommentId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, NewCommentReact react)
        {
            var query = @"UPDATE CommentReacts SET ReactType = @ReactType WHERE Id = @Id";
            var parameters = new DynamicParameters();
            //parameters.Add("ReactType", react.ReactType, DbType.Int32);
            parameters.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok(result);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update CommentReacts SET isDeleted = 1 WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }

    }
}

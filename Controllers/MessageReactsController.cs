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
    public class MessageReactsController : ControllerBase
    {
        private readonly DapperContext _context;

        public MessageReactsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("comment/{commentId}")]
        public async Task<IEnumerable<MessageReactQuery>> GetByCommentId(Guid commentId)
        {
            var query = @"SELECT m.Id AS ReactId, m.ReactType AS Type, m.UserId AS ReactUserId, u.UserName AS ReactUserName, u.AvatarUrl AS ReactUserAvatar
                          FROM MessageReacts m
                          LEFT JOIN User u ON u.Id = m.UserId AND m.IsDeleted = 0 AND m.CommentId = @CommentId";
            var parameter = new DynamicParameters();
            parameter.Add("CommentId", commentId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<MessageReactQuery>(query, parameter);
                return result;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateByCommentId(NewMessageReact react)
        {
            var query = @"INSERT INTO MessageReacts (Id, CreatedAt, UpdatedAt, IsDeleted, ReactType, UserId, PostId)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @ReactType, @UserId, @CommentId)";
            var parameters = new DynamicParameters();
            //parameters.Add("ReactType", react.ReactType, DbType.Int32);
            parameters.Add("UserId", react.UserId, DbType.Guid);
            parameters.Add("CommentId", react.MessageId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, NewMessageReact react)
        {
            var query = @"UPDATE MessageReacts SET ReactType = @ReactType WHERE Id = @Id";
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
            var query = "Update MessageReacts SET isDeleted = 1 WHERE Id = @Id";
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

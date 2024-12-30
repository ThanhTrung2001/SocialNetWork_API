using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly DapperContext _context;

        public MessagesController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("chatbox/{chatBoxId}")]
        public async Task<IEnumerable<MessageQuery>> GetByPostID(Guid chatBoxId)
        {
            var query = @"SELECT m.Id, m.Content, m.CreatedAt, u.Id as UserId, u.UserName, u.AvatarUrl 
                          FROM Messages m 
                          INNER JOIN Users u ON m.UserId = u.Id
                          WHERE m.ChatBoxId = @Id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<MessageQuery>(query, new { Id = chatBoxId });
                return result;
            }
        }

        [HttpPost("chatbox/{chatBoxId}")]
        public async Task<IActionResult> AddNewMessage(Guid chatBoxId, NewMessage message)
        {
            var query = @"INSERT INTO Messages (Id, CreatedAt, UpdatedAt, IsDeleted, Content, UserId, ChatBoxId)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @Content, @UserId, @ChatBoxId);";
            var parameters = new DynamicParameters();
            parameters.Add("Content", message.Content, DbType.String);
            parameters.Add("UserId", message.UserId, DbType.Guid);
            parameters.Add("ChatBoxId", chatBoxId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessage(Guid id, NewMessage message)
        {
            var query = "UPDATE Messages SET Content = @Content WHERE Id = @Id AND UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("Content", message.Content, DbType.String);
            parameters.Add("UserId", message.UserId, DbType.Guid);
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
            var query = "Update Messages SET isDeleted = 1 WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }


    }
}

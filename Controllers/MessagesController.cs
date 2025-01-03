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
    public class MessagesController : ControllerBase
    {
        private readonly DapperContext _context;

        public MessagesController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("chatbox/{chatBoxId}")]
        public async Task<IEnumerable<MessageQuery>> GetByChatBoxID(Guid chatBoxId)
        {
            var query = @"SELECT m.Id, m.Content, m.CreatedAt, m.IsPinned, u.Id as UserId, u.UserName, u.AvatarUrl, m.ReactCount
                          FROM Messages m 
                          INNER JOIN Users u ON m.UserId = u.Id
                          WHERE m.ChatBoxId = @Id AND m.IsDeleted = 0";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<MessageQuery>(query, new { Id = chatBoxId });
                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<MessageDetailQuery> GetByID(Guid id)
        {
            var query = @"SELECT m.Id, m.Content, m.MediaURL, m.UpdatedAt, m.UserId, u.Username, u.AvatarUrl, mr.Id AS ReactId, mr.ReactType, mr.UserId AS ReactUserId, umr.UserName AS ReactUserName, umr.AvatarUrl AS ReactUserAvatar 
                          FROM Messages m
                          INNER JOIN Users u ON m.UserId = u.Id 
                          LEFT JOIN MessageReacts mr ON m.Id = mr.MessageId
                          LEFT JOIN Users umr ON mr.UserId = umr.Id
                          Where c.Id = @Id & c.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var messageResult = new MessageDetailQuery();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<MessageDetailQuery, MessageReactQuery, MessageDetailQuery>(
                    query,
                    map: (message, react) =>
                    {
                        if (message != null)
                        {
                            messageResult = message;
                        }

                        if (react != null && !messageResult.Reacts.Any((item) => item.ReactId == react.ReactId))
                        {
                            messageResult.Reacts.Add(react);
                        }
                        return messageResult;
                    },
                    parameter,
                    splitOn: "ReactId");
                    return messageResult;
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("chatbox/{chatBoxId}")]
        public async Task<IActionResult> AddNewMessage(Guid chatBoxId, NewMessage message)
        {
            var query = @"INSERT INTO Messages (Id, CreatedAt, UpdatedAt, IsDeleted, Content, UserId, ChatBoxId, IsPinned)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @Content, @UserId, @ChatBoxId, 0);";
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

        [HttpPut("pin/{id}")]
        public async Task<IActionResult> PinMessage(Guid id, NewMessage message)
        {
            var query = "UPDATE Messages SET IsPinned = 1 WHERE Id = @Id AND UserId = @UserId";
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

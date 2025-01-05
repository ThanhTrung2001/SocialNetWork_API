using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Models.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
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

        [HttpGet("chatgroup/{chatGroupId}")]
        public async Task<IEnumerable<MessageQuery>> GetByChatGroupID(Guid chatGroupId)
        {
            var query = @"SELECT 
                          m.Id, m.CreatedAt, m.Content, m.IsPinned, m.SenderId, m.ChatGroupId, m.IsPinned, m.IsResponse, m.TypeId AS MessageType, m.StatusId,
                          ud.FirstName, ud.LastName, ud.Avatar
                          FROM Messages m 
                          INNER JOIN UserDetails ud ON m.SenderId = ud.UserId
                          WHERE m.ChatGroupId = @Id AND m.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("ChatGroupId", chatGroupId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<MessageQuery>(query, parameter);
                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<MessageDetailQuery> GetByID(Guid id)
        {
            var query = @"SELECT 
                          m.Id, m.CreatedAt, m.Content, m.IsPinned, m.SenderId, m.ChatGroupId, m.IsPinned, m.IsResponse, m.TypeId AS MessageType, m.StatusId,
                          ud.FirstName, ud.LastName, ud.Avatar,
                          urm.ReactTypeId AS ReactId, urm.UserId as ReactUserId, urm.CreatedAt, 
                          r.TypeName,
                          ur.FirstName AS ReactFirstName, ur.LastName AS ReactLastName, ur.Avatar AS ReactAvatar
                          FROM Messages m 
                          INNER JOIN UserDetails ud ON m.SenderId = ud.UserId
                          LEFT JOIN UserReactMessage urm ON m.Id = urm.MessageId
                          LEFT JOIN React r ON r.Id = urm.ReactTypeId
                          LEFT JOIN UserDetails ur ON urm.UserId = ur.UserId
                          WHERE m.Id = @Id AND m.IsDeleted = 0";
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

                        if (react != null && !messageResult.Reacts.Any((item) => item.ReactUserId == react.ReactUserId))
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

        [HttpPost("chatGroup/{chatGroupId}")]
        public async Task<IActionResult> AddNewMessage(Guid chatGroupId, CreateMessageCommand message)
        {
            var query = @"INSERT INTO Messages (Id, CreatedAt, UpdatedAt, IsDeleted, Content, SenderId, ChatGroupId, IsPinned, IsResponse, ReactCount, TypeId, StatusId)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @Content, @SenderId, @ChatGroupId, 0, @IsResponse, 0, @TypeId, 1);";
            var parameters = new DynamicParameters();
            parameters.Add("Content", message.Content, DbType.String);
            parameters.Add("SenderId", message.SenderId, DbType.Guid);
            parameters.Add("ChatGroupId", chatGroupId, DbType.Guid);
            parameters.Add("IsReponse", message.IsResponse);
            parameters.Add("TypeId", message.MessageTypeId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessage(Guid id, CreateMessageCommand message)
        {
            var query = "UPDATE Messages SET Content = @Content, UpdatedAt = GETDATE() WHERE Id = @Id AND SenderId = @SenderId";
            var parameters = new DynamicParameters();
            parameters.Add("Content", message.Content, DbType.String);
            parameters.Add("SenderId", message.SenderId, DbType.Guid);
            parameters.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpPut("pin/{id}")]
        public async Task<IActionResult> PinMessage(Guid id, CreateMessageCommand message)
        {
            var query = "UPDATE Messages SET IsPinned = 1, UpdatedAt = GETDATE() WHERE Id = @Id AND SenderId = @SenderId";
            var parameters = new DynamicParameters();
            parameters.Add("SenderId", message.SenderId, DbType.Guid);
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
            var query = "Update Messages SET isDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }


    }
}

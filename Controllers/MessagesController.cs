using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[Authorize]
    [AllowAnonymous]
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
                          ud.FirstName, ud.LastName, ud.Avatar,
                          
                          a.Id AS AttachmentId,
                          a.Media,
                          a.Description
                          
                          FROM Messages m 
                          INNER JOIN 
                            UserDetails ud ON m.SenderId = ud.UserId
                          LEFT JOIN
                            MessageAttachment ma ON ma.MessageId = m.Id
                          LEFT JOIN
                            Attachments a ON ma.AttachmentId = a.Id
                          WHERE m.ChatGroupId = @Id AND m.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", chatGroupId);
            try
            {
                var messageDict = new Dictionary<Guid, MessageQuery>();
                using (var connection = _context.CreateConnection())
                {

                    var result = await connection.QueryAsync<MessageQuery, AttachmentQuery, MessageQuery>(
                        query,
                    map: (message, attachment) =>
                    {
                        if (!messageDict.TryGetValue(message.Id, out var messageEntry))
                        {
                            messageEntry = message;
                            messageDict.Add(message.Id, messageEntry);
                        }

                        if (attachment != null && !message.Attachments.Any((item) => item.AttachmentId == attachment.AttachmentId))
                        {
                            messageEntry.Attachments.Add(attachment);
                        }
                        return messageEntry;
                    },
                        parameter,
                        splitOn: "AttachmentId");
                    return messageDict.Values.ToList();
                }

            }
            catch (Exception ex)
            {
                throw ex;
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
                          ur.FirstName AS ReactFirstName, ur.LastName AS ReactLastName, ur.Avatar AS ReactAvatar,

                          a.Id AS AttachmentId,
                          a.Media,
                          a.Description

                          FROM Messages m 
                          INNER JOIN UserDetails ud ON m.SenderId = ud.UserId
                          LEFT JOIN UserReactMessage urm ON m.Id = urm.MessageId
                          LEFT JOIN ReactTypes r ON r.Id = urm.ReactTypeId
                          LEFT JOIN UserDetails ur ON urm.UserId = ur.UserId
                          LEFT JOIN
                            MessageAttachment ma ON ma.MessageId = m.Id
                          LEFT JOIN
                            Attachments a ON ma.AttachmentId = a.Id
                          WHERE m.Id = @Id AND m.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var messageDict = new Dictionary<Guid, MessageDetailQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<MessageDetailQuery, MessageReactQuery, AttachmentQuery, MessageDetailQuery>(
                    query,
                    map: (message, react, attachment) =>
                    {

                        if (!messageDict.TryGetValue(message.Id, out var messageEntry))
                        {
                            messageEntry = message;
                            messageDict.Add(message.Id, messageEntry);
                        }

                        if (react != null && !messageEntry.Reacts.Any((item) => item.ReactUserId == react.ReactUserId))
                        {
                            messageEntry.Reacts.Add(react);
                        }
                        if (attachment != null && !messageEntry.Attachments.Any((item) => item.AttachmentId == attachment.AttachmentId))
                        {
                            messageEntry.Attachments.Add(attachment);
                        }
                        return messageEntry;
                    },
                    parameter,
                    splitOn: "ReactId, AttachmentId");
                    return messageDict.Values.ToList()[0];
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
                        OUTPUT Inserted.Id
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @Content, @SenderId, @ChatGroupId, 0, @IsResponse, 0, @TypeId, 1);";
            var queryAttachment = @"INSERT INTO Attachments (Id, Media, Description)
                                    OUTPUT Inserted.Id
                                    VALUES
                                    (NEWID(), @Media, @Description)";
            var queryMessAttachment = @"INSERT INTO MessageAttachment (MessageId, AttachmentId)
                                        VALUES
                                        (@MessageId, @AttachmentId)";
            var parameters = new DynamicParameters();
            parameters.Add("Content", message.Content, DbType.String);
            parameters.Add("SenderId", message.SenderId, DbType.Guid);
            parameters.Add("ChatGroupId", chatGroupId, DbType.Guid);
            parameters.Add("IsResponse", message.IsResponse);
            parameters.Add("TypeId", message.MessageTypeId);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = await connection.QuerySingleAsync<Guid>(query, parameters, transaction);
                        if (message.Attachments != null)
                        {
                            foreach (var item in message.Attachments!)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("Media", item.Media);
                                parameters.Add("Description", item.Description);
                                var attachmentResult = await connection.QuerySingleAsync<Guid>(queryAttachment, parameters, transaction);
                                parameters.Add("MessageId", result);
                                parameters.Add("AttachmentId", attachmentResult);
                                await connection.ExecuteAsync(queryMessAttachment, parameters, transaction);
                            }

                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
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

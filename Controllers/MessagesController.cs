using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
using EnVietSocialNetWorkAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly DapperContext _context;

        public MessagesController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("chatgroup/{ChatGroup_Id}")]
        public async Task<IActionResult> GetByChatGroup_Id(Guid ChatGroup_Id)
        {
            var query = @"SELECT 
                          m.Id, m.Created_At, m.Content, m.Sender_Id, m.ChatGroup_Id, m.Is_Pinned, m.Is_Response, m.Type, m.Status,
                          ud.FirstName, ud.LastName, ud.Avatar,
                          
                          a.Id AS Attachment_Id,
                          a.Media,
                          a.Description
                          
                          FROM Messages m 
                          INNER JOIN 
                            User_Details ud ON m.Sender_Id = ud.User_Id
                          LEFT JOIN
                            Message_Attachment ma ON ma.Message_Id = m.Id
                          LEFT JOIN
                            Attachments a ON ma.Attachment_Id = a.Id
                          WHERE m.ChatGroup_Id = @Id AND m.Is_Deleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", ChatGroup_Id);
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

                            if (attachment != null && !message.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                            {
                                messageEntry.Attachments.Add(attachment);
                            }
                            return messageEntry;
                        },
                        parameter,
                        splitOn: "Attachment_Id");
                    return Ok(ResponseModel<IEnumerable<MessageQuery>>.Success(messageDict.Values.ToList()));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<MessageQuery>>.Failure(ex.Message));
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var query = @"SELECT 
                          m.Id, m.Created_At, m.Content, m.Is_Pinned, m.Sender_Id, m.ChatGroup_Id, m.Is_Pinned, m.Is_Response, m.Type, m.Status,
                          ud.FirstName, ud.LastName, ud.Avatar,

                          urm.React_Type, urm.User_Id as React_UserId, urm.Created_At, 
                          ur.FirstName AS React_FirstName, ur.LastName AS React_LastName, ur.Avatar AS React_Avatar,

                          a.Id AS Attachment_Id,
                          a.Media,
                          a.Description

                          FROM Messages m 
                          INNER JOIN User_Details ud ON m.Sender_Id = ud.User_Id
                          LEFT JOIN User_React_Message urm ON m.Id = urm.Message_Id
                          LEFT JOIN User_Details ur ON urm.User_Id = ur.User_Id
                          LEFT JOIN
                            Message_Attachment ma ON ma.Message_Id = m.Id
                          LEFT JOIN
                            Attachments a ON ma.Attachment_Id = a.Id
                          WHERE m.Id = @Id AND m.Is_Deleted = 0";
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

                        if (react != null && !messageEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                        {
                            messageEntry.Reacts.Add(react);
                        }
                        if (attachment != null && !messageEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            messageEntry.Attachments.Add(attachment);
                        }
                        return messageEntry;
                    },
                    parameter,
                    splitOn: "React_Type, Attachment_Id");
                    return Ok(ResponseModel<MessageDetailQuery>.Success(messageDict.Values.ToList()[0]));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<MessageDetailQuery>.Failure(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewMessage(CreateMessageCommand message)
        {
            var query = @"INSERT INTO Messages (Id, Created_At, Updated_At, Is_Deleted, Content, Sender_Id, ChatGroup_Id, Is_Pinned, Is_Response, React_Count, Type, Status)
                        OUTPUT Inserted.Id
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @Content, @Sender_Id, @ChatGroup_Id, 0, @Is_Response, 0, @Type, 'Send');";
            var queryAttachment = @"INSERT INTO Attachments (Id, Media, Description)
                                    OUTPUT Inserted.Id
                                    VALUES
                                    (NEWID(), @Media, @Description);";
            var queryMessAttachment = @"INSERT INTO Message_Attachment (Message_Id, Attachment_Id)
                                        VALUES
                                        (@Message_Id, @Attachment_Id)";
            var queryResponse = @"INSERT INTO Message_Response (Message_Id, Response_Id)
                                  VALUES
                                  (@Message_Id, @Response_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("Content", message.Content, DbType.String);
            parameters.Add("Sender_Id", message.Sender_Id, DbType.Guid);
            parameters.Add("ChatGroup_Id", message.ChatGroup_Id, DbType.Guid);
            parameters.Add("Is_Response", message.Is_Response);
            parameters.Add("Type", message.Type);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = await connection.QuerySingleAsync<Guid>(query, parameters, transaction);

                        foreach (var item in message.Attachments!)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("Media", item.Media);
                            parameters.Add("Description", item.Description);
                            var attachmentResult = await connection.QuerySingleAsync<Guid>(queryAttachment, parameters, transaction);
                            parameters.Add("Message_Id", result);
                            parameters.Add("Attachment_Id", attachmentResult);
                            await connection.ExecuteAsync(queryMessAttachment, parameters, transaction);
                        }

                        if (message.Is_Response == true)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("Message_Id", message.Message_Id);
                            parameters.Add("Response_Id", result);
                            await connection.ExecuteAsync(queryResponse, parameters, transaction);
                        }
                        transaction.Commit();
                        return Ok(ResponseModel<Guid>.Success(result));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<Guid>.Failure(ex.Message));
                    }
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessage(Guid id, EditMessageCommand message)
        {
            var query = "UPDATE Messages SET Content = @Content, Updated_At = GETDATE() WHERE Id = @Id AND Sender_Id = @Sender_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Content", message.Content, DbType.String);
            parameters.Add("Sender_Id", message.Sender_Id, DbType.Guid);
            parameters.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return Ok(ResponseModel<string>.Success("Success."));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpPut("{id}/pin")]
        public async Task<IActionResult> PinMessage(Guid id)
        {
            var query = "UPDATE Messages SET Is_Pinned = 1, Updated_At = GETDATE() WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameter);
                    return Ok(ResponseModel<string>.Success("Success."));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update Messages SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id";
            var queryReact = "Update User_React_Message SET  Is_Deleted = 1, Updated_At = GETDATE() WHERE Message_Id = @Id";
            var queryAttachment = "Update Attachments SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id IN (SELECT Attachment_Id FROM Message_Attachment WHERE Message_Id = @Id)";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameter, transaction);
                        await connection.ExecuteAsync(queryReact, parameter, transaction);
                        await connection.ExecuteAsync(queryAttachment, parameter, transaction);
                        transaction.Commit();
                        return Ok(ResponseModel<string>.Success("Success."));

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<string>.Failure(ex.Message));
                    }
                }
            }
        }


    }
}
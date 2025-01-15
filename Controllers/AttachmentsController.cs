using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
using EnVietSocialNetWorkAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly DapperContext _context;
        public AttachmentsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("post/{id}")]
        public async Task<IActionResult> GetAttachmentsByPostId(Guid id)
        {
            var query = @"SELECT pa.Attachment_Id, a.Media, a.Description
                          FROM Post_Attachment pa
                          INNER JOIN Attachments a ON pa.Attachment_Id = a.Id
                          WHERE pa.Post_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<AttachmentQuery>(query, parameter);
                    return Ok(ResponseModel<IEnumerable<AttachmentQuery>>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<AttachmentQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpGet("comment/{id}")]
        public async Task<IActionResult> GetAttachmentsByCommentId(Guid id)
        {
            var query = @"SELECT ca.Attachment_Id, a.Media, a.Description
                          FROM Comment_Attachment ca
                          INNER JOIN Attachments a ON ca.Attachment_Id = a.Id
                          WHERE ca.Comment_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<AttachmentQuery>(query, parameter);
                    return Ok(ResponseModel<IEnumerable<AttachmentQuery>>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<AttachmentQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpGet("message/{id}")]
        public async Task<IActionResult> GetAttachmentsByMessageId(Guid id)
        {
            var query = @"SELECT ma.Attachment_Id, a.Media, a.Description
                          FROM Message_Attachment ma
                          INNER JOIN Attachments a ON ma.Attachment_Id = a.Id
                          WHERE ma.Message_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<AttachmentQuery>(query, parameter);
                    return Ok(ResponseModel<IEnumerable<AttachmentQuery>>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<AttachmentQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpPost("post/{id}")]
        public async Task<IActionResult> CreatePostAttachment(Guid id, CreateAttachmentListCommand command)
        {
            var queryAttachment = @"INSERT INTO Attachments (Id, Created_At, Updated_At, Is_Deleted, Media, Description)
                               OUTPUT Inserted.Id
                               VALUES 
                               (NEWID(), GETDATE(), GETDATE(), 0, @Media, @Description)";
            var queryPost_Attachment = @"INSERT INTO Post_Attachment (Post_Id, Attachment_Id)
                                       VALUES 
                                       (@Post_Id, @Attachment_Id)";
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (command.Attachments != null)
                        {
                            foreach (var item in command.Attachments)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("Post_Id", id);
                                parameters.Add("Media", item.Media);
                                parameters.Add("Description", item.Description);
                                var result = await connection.QuerySingleAsync<Guid>(queryAttachment, parameters, transaction);
                                parameters.Add("Attachment_Id", result);
                                await connection.ExecuteAsync(queryPost_Attachment, parameters, transaction);
                            }

                        }
                        transaction.Commit();
                        return Ok(ResponseModel<string>.Success("Success"));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<string>.Failure(ex.Message));
                    }
                }
            }
        }

        [HttpPost("comment/{id}")]
        public async Task<IActionResult> CreateCommentAttachment(Guid id, CreateAttachmentListCommand command)
        {
            var queryAttachment = @"INSERT INTO Attachments (Id, Created_At, Updated_At, Is_Deleted, Media, Description)
                               OUTPUT Inserted.Id
                               VALUES 
                               (NEWID(), GETDATE(), GETDATE(), 0, @Media, @Description)";
            var queryComment_Attachment = @"INSERT INTO Comment_Attachment (Comment_Id, Attachment_Id)
                                       VALUES 
                                       (@Comment_Id, @Attachment_Id)";
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (command.Attachments != null)
                        {
                            foreach (var item in command.Attachments)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("Comment_Id", id);
                                parameters.Add("Media", item.Media);
                                parameters.Add("Description", item.Description);
                                var result = await connection.QuerySingleAsync<Guid>(queryAttachment, parameters, transaction);
                                parameters.Add("Attachment_Id", result);
                                await connection.ExecuteAsync(queryComment_Attachment, parameters, transaction);
                            }

                        }
                        transaction.Commit();
                        return Ok(ResponseModel<string>.Success("Success"));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<string>.Failure(ex.Message));
                    }
                }
            }
        }

        [HttpPost("message/{id}")]
        public async Task<IActionResult> CreateMessageAttachment(Guid id, CreateAttachmentListCommand command)
        {
            var queryAttachment = @"INSERT INTO Attachments (Id, Created_At, Updated_At, Is_Deleted, Media, Description)
                               OUTPUT Inserted.Id
                               VALUES 
                               (NEWID(), GETDATE(), GETDATE(), 0, @Media, @Description)";
            var queryMessage_Attachment = @"INSERT INTO Message_Attachment (Message_Id, Attachment_Id)
                                       VALUES 
                                       (@Message_Id, @Attachment_Id)";
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (command.Attachments != null)
                        {
                            foreach (var item in command.Attachments)
                            {
                                var parameters = new DynamicParameters();
                                parameters.Add("Message_Id", id);
                                parameters.Add("Media", item.Media);
                                parameters.Add("Description", item.Description);
                                var result = await connection.QuerySingleAsync<Guid>(queryAttachment, parameters, transaction);
                                parameters.Add("Attachment_Id", result);
                                await connection.ExecuteAsync(queryMessage_Attachment, parameters, transaction);
                            }

                        }
                        transaction.Commit();
                        return Ok(ResponseModel<string>.Success("Success"));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<string>.Failure(ex.Message));
                    }
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditAttachment(Guid id, CreateAttachmentCommand command)
        {
            var query = "UPDATE Attachments SET Media = @Media, Description = @Description WHERE Id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Media", command.Media);
            parameters.Add("Description", command.Description);
            parameters.Add("Id", id);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttachment(Guid id)
        {
            var query = "DELETE Attachments WHERE ID = @Id";
            var queryPost = "DELETE Post_Attachment WHERE Attachment_Id = @Id";
            var queryComment = "DELETE Comment_Attachment WHERE Attachment_Id = @Id";
            var queryMessage = "DELETE Message_Attachment WHERE Attachment_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(queryPost, parameter, transaction);
                        await connection.ExecuteAsync(queryComment, parameter, transaction);
                        await connection.ExecuteAsync(queryMessage, parameter, transaction);
                        await connection.ExecuteAsync(query, parameter, transaction);
                        //Delete online files

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

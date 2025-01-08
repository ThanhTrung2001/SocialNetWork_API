﻿using Dapper;
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
    public class CommentsController : ControllerBase
    {
        private readonly DapperContext _context;
        public CommentsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("post/{post_Id}")]
        public async Task<IEnumerable<CommentQuery>> GetCommentsByPost_Id(Guid post_Id)
        {
            var query = @"SELECT 
                            c.Id, c.Content, c.Is_Response, c.Is_SharePost,c.React_Count, c.Updated_At, c.User_Id,
                            ud.FirstName, ud.LastName, ud.Avatar,
                            a.Id AS Attachment_Id, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN User_Details ud ON c.User_Id = ud.User_Id 
                          LEFT JOIN
                            Comment_Attachment ca ON ca.Comment_Id = c.Id
                          LEFT JOIN
                            Attachments a ON ca.Attachment_Id = a.Id
                          Where c.Post_Id = @Id AND c.Is_Deleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", post_Id);
            try
            {
                var commentDict = new Dictionary<Guid, CommentQuery>();
                using (var connection = _context.CreateConnection())
                {

                    var result = await connection.QueryAsync<CommentQuery, AttachmentQuery, CommentQuery>(
                        query,
                    map: (comment, attachment) =>
                    {
                        if (!commentDict.TryGetValue(comment.Id, out var commentEntry))
                        {
                            commentEntry = comment;
                            commentDict.Add(comment.Id, commentEntry);
                        }

                        if (attachment != null && !commentEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            commentEntry.Attachments.Add(attachment);
                        }
                        return commentEntry;
                    },
                        parameter,
                        splitOn: "Attachment_Id");
                    return commentDict.Values.ToList();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("detail/post/{Post_Id}")]
        public async Task<IEnumerable<CommentDetailQuery>> GetCommentDetailsByPost_Id(Guid Post_Id)
        {
            var query = @"SELECT 
                          c.Id, c.Content, c.Is_Response, c.Is_SharePost, c.React_Count, c.Updated_At, c.User_Id, 
                          ud.FirstName, ud.LastName, ud.Avatar,
                          urc.React_Type, urc.User_Id as React_User_Id, urc.CreatedAt, 
                          ur.FirstName AS React_FirstName, ur.LastName AS React_LastName, ur.Avatar AS React_Avatar,
                          a.Id AS Attachment_Id, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN User_Details ud ON c.User_Id = ud.User_Id 
                          LEFT JOIN User_React_Comment urc ON c.Id = urc.Comment_Id
                          LEFT JOIN User_Details ur ON urc.User_Id = ur.User_Id
                          LEFT JOIN
                            Comment_Attachment ca ON ca.Comment_Id = c.Id
                          LEFT JOIN
                            Attachments a ON ca.Attachment_Id = a.Id
                          Where c.Post_Id = @Id AND c.Is_Deleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", Post_Id);
            try
            {
                var commentDict = new Dictionary<Guid, CommentDetailQuery>();
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<CommentDetailQuery, CommentReactQuery, AttachmentQuery, CommentDetailQuery>(query,
                        map: (comment, react, attachment) =>
                        {
                            if (!commentDict.TryGetValue(comment.Id, out var commentEntry))
                            {
                                commentEntry = comment;
                                commentDict.Add(comment.Id, commentEntry);
                            }
                            if (react != null && !commentEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                            {
                                commentEntry.Reacts.Add(react);
                            }
                            if (attachment != null && !commentEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                            {
                                commentEntry.Attachments.Add(attachment);
                            }
                            return commentEntry;
                        },
                        parameter,
                        splitOn: "React_Type, Attachment_Id");
                    return commentDict.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{id}")]
        public async Task<CommentDetailQuery> GetCommentByID(Guid id)
        {
            var query = @"SELECT 
                          c.Id, c.Content, c.Is_Response, c.React_Count, c.Updated_At, c.User_Id, 
                          ud.FirstName, ud.LastName, ud.Avatar,
                          urc.React_Type, urc.User_Id as React_UserId, urc.CreatedAt,
                          ur.FirstName AS React_FirstName, ur.LastName AS React_LastName, ur.Avatar AS React_Avatar,
                          a.Id AS Attachment_Id, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN User_Details ud ON c.User_Id = ud.User_Id 
                          LEFT JOIN User_React_Comment urc ON c.Id = urc.Comment_Id
                          LEFT JOIN User_Details ur ON urc.User_Id = ur.User_Id
                          LEFT JOIN
                            Comment_Attachment ca ON ca.Comment_Id = c.Id
                          LEFT JOIN
                            Attachments a ON ca.Attachment_Id = a.Id
                          Where c.Id = @Id AND c.Is_Deleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var commentDict = new Dictionary<Guid, CommentDetailQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<CommentDetailQuery, CommentReactQuery, AttachmentQuery, CommentDetailQuery>(
                    query,
                    map: (comment, react, attachment) =>
                    {
                        if (!commentDict.TryGetValue(comment.Id, out var commentEntry))
                        {
                            commentEntry = comment;
                            commentDict.Add(comment.Id, commentEntry);
                        }

                        if (react != null && !commentEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                        {
                            commentEntry.Reacts.Add(react);
                        }
                        if (attachment != null && !commentEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            commentEntry.Attachments.Add(attachment);
                        }
                        return commentEntry;
                    },
                    parameter,
                    splitOn: "React_Type, Attachment_Id");
                }
                return commentDict.Values.ToList()[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{id}/reponse")]
        public async Task<IEnumerable<CommentDetailQuery>> GetCommentResponseByID(Guid id)
        {
            var query = @"SELECT 
                          c.Id, c.Content, c.Is_Response, c.React_Count, c.Updated_At, c.User_Id, 
                          ud.FirstName, ud.LastName , ud.Avatar,
                          urc.React_Type, urc.User_Id as React_UserId, urc.CreatedAt, 
                          ur.FirstName AS React_FirstName, ur.LastName AS React_LastName, ur.Avatar AS React_Avatar,
                          a.Id AS Attachment_Id, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN User_Details ud ON c.User_Id = ud.User_Id 
                          LEFT JOIN User_React_Comment urc ON c.Id = urc.Comment_Id
                          LEFT JOIN User_Details ur ON urc.User_Id = ur.User_Id
                          LEFT JOIN CommentResponse cmr ON c.Id = cmr.Response_Id
                          LEFT JOIN
                            Comment_Attachment ca ON ca.Comment_Id = c.Id
                          LEFT JOIN
                            Attachments a ON ca.Attachment_Id = a.Id
                          Where c.Is_Deleted = 0 AND c.Is_Response = 1 AND cmr.Comment_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var commentDict = new Dictionary<Guid, CommentDetailQuery>();
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<CommentDetailQuery, CommentReactQuery, AttachmentQuery, CommentDetailQuery>(query,
                        map: (comment, react, attachment) =>
                        {
                            if (!commentDict.TryGetValue(comment.Id, out var commentEntry))
                            {
                                commentEntry = comment;
                                commentDict.Add(comment.Id, commentEntry);
                            }
                            if (react != null && !commentEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                            {
                                commentEntry.Reacts.Add(react);
                            }
                            if (attachment != null && !commentEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                            {
                                commentEntry.Attachments.Add(attachment);
                            }
                            return commentEntry;
                        },
                        parameter,
                        splitOn: "React_Type, Attachment_Id");
                    return commentDict.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("post/{post_Id}")]
        public async Task<IActionResult> CreateComment(Guid post_Id, CreateCommentCommand comment)
        {
            var query = @"INSERT INTO Comments (Id, CreatedAt, Updated_At, Is_Deleted, Content, Is_Response, React_Count ,User_Id, Post_Id)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Content, @Is_Response, 0, @User_Id, @Post_Id)";
            var queryAttachment = @"INSERT INTO Attachments (Id, Media, Description)
                                    OUTPUT Inserted.Id
                                    VALUES
                                    (NEWID(), @Media, @Description)";
            var queryComment_Attachment = @"INSERT INTO Comment_Attachment (Comment_Id, Attachment_Id)
                                        VALUES
                                        (@Comment_Id, @Attachment_Id)";
            var queryResponse = @"INSERT INTO CommentResponse (Comment_Id, Response_Id)
                                  VALUES
                                  (@Comment_Id, @Response_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("Content", comment.Content, DbType.String);
            parameters.Add("User_Id", comment.User_Id, DbType.Guid);
            parameters.Add("Post_Id", post_Id);
            parameters.Add("Is_Response", comment.Is_Response);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = await connection.QuerySingleAsync<Guid>(query, parameters, transaction);
                        if (comment.Attachments != null)
                        {
                            foreach (var item in comment.Attachments!)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("Media", item.Media);
                                parameters.Add("Description", item.Description);
                                var attachmentResult = await connection.QuerySingleAsync<Guid>(queryAttachment, parameters, transaction);
                                parameters.Add("Comment_Id", result);
                                parameters.Add("Attachment_Id", attachmentResult);
                                await connection.ExecuteAsync(queryComment_Attachment, parameters, transaction);
                            }
                        }
                        if (comment.Is_Response == true)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("Comment_Id", comment.Comment_Id);
                            parameters.Add("Response_Id", result);
                            await connection.ExecuteAsync(queryResponse, parameters, transaction);
                        }

                        transaction.Commit();
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditComment(Guid id, EditCommentCommand comment)
        {
            var query = "UPDATE Comments SET Content = @Content, Updated_At = GETDATE() WHERE Id = @Id AND User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Content", comment.Content, DbType.String);
            parameters.Add("User_Id", comment.User_Id, DbType.Guid);
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
            var query = "Update Comments SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }
    }
}
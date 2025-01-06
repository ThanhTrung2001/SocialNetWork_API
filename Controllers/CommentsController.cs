using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Models.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
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

        [HttpGet("post/{postId}")]
        public async Task<IEnumerable<CommentQuery>> GetCommentsByPostID(Guid postId)
        {
            var query = @"SELECT 
                            c.Id, c.Content, c.IsResponse, c.ReactCount, c.UpdatedAt, c.UserId,
                            ud.FirstName AS UserFirstName, ud.LastName AS UserLastName, ud.Avatar,
                            a.Id AS AttachmentId, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN UserDetails ud ON c.UserId = ud.UserId 
                          LEFT JOIN
                            CommentAttachment ca ON ca.CommentId = c.Id
                          LEFT JOIN
                            Attachments a ON ca.AttachmentId = a.Id
                          Where c.PostId = @Id AND c.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", postId);

            //using (var connection = _context.CreateConnection())
            //{
            //    var result = await connection.QueryAsync<CommentQuery>(query, parameter);

            //    return result;
            //}
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

                        if (attachment != null && !commentEntry.Attachments.Any((item) => item.AttachmentId == attachment.AttachmentId))
                        {
                            commentEntry.Attachments.Add(attachment);
                        }
                        return commentEntry;
                    },
                        parameter,
                        splitOn: "AttachmentId");
                    return commentDict.Values.ToList();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("detail/post/{postId}")]
        public async Task<IEnumerable<CommentDetailQuery>> GetCommentDetailsByPostID(Guid postId)
        {
            var query = @"SELECT 
                          c.Id, c.Content, c.IsResponse, c.ReactCount, c.UpdatedAt, c.UserId, 
                          ud.FirstName AS UserFirstName, ud.LastName AS UserLastName ,ud.Avatar,
                          urc.ReactTypeId AS ReactId, urc.UserId as ReactUserId, urc.CreatedAt, 
                          r.TypeName,
                          ur.FirstName AS ReactFirstName, ur.LastName AS ReactLastName, ur.Avatar AS ReactAvatar,
                          a.Id AS AttachmentId, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN UserDetails ud ON c.UserId = ud.UserId 
                          LEFT JOIN UserReactComment urc ON c.Id = urc.CommentId
                          LEFT JOIN ReactTypes r ON r.Id = urc.ReactTypeId
                          LEFT JOIN UserDetails ur ON urc.UserId = ur.UserId
                          LEFT JOIN
                            CommentAttachment ca ON ca.CommentId = c.Id
                          LEFT JOIN
                            Attachments a ON ca.AttachmentId = a.Id
                          Where c.PostId = @Id AND c.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", postId);
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
                            if (react != null && !commentEntry.Reacts.Any((item) => item.ReactUserId == react.ReactUserId))
                            {
                                commentEntry.Reacts.Add(react);
                            }
                            if (attachment != null && !commentEntry.Attachments.Any((item) => item.AttachmentId == attachment.AttachmentId))
                            {
                                commentEntry.Attachments.Add(attachment);
                            }
                            return commentEntry;
                        },
                        parameter,
                        splitOn: "ReactUserId, AttachmentId");
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
                          c.Id, c.Content, c.IsResponse, c.ReactCount, c.UpdatedAt, c.UserId, 
                          ud.FirstName, ud.LastName, ud.Avatar,
                          urc.ReactTypeId AS ReactId, urc.UserId as ReactUserId, urc.CreatedAt, 
                          r.TypeName,
                          ur.FirstName AS ReactFirstName, ur.LastName AS ReactLastName, ur.Avatar AS ReactAvatar,
                          a.Id AS AttachmentId, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN UserDetails ud ON c.UserId = ud.UserId 
                          LEFT JOIN UserReactComment urc ON c.Id = urc.CommentId
                          LEFT JOIN ReactTypes r ON r.Id = urc.ReactTypeId
                          LEFT JOIN UserDetails ur ON urc.UserId = ur.UserId
                          LEFT JOIN
                            CommentAttachment ca ON ca.CommentId = c.Id
                          LEFT JOIN
                            Attachments a ON ca.AttachmentId = a.Id
                          Where c.Id = @Id AND c.IsDeleted = 0";
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

                        if (react != null && !commentEntry.Reacts.Any((item) => item.ReactUserId == react.ReactUserId))
                        {
                            commentEntry.Reacts.Add(react);
                        }
                        if (attachment != null && !commentEntry.Attachments.Any((item) => item.AttachmentId == attachment.AttachmentId))
                        {
                            commentEntry.Attachments.Add(attachment);
                        }
                        return commentEntry;
                    },
                    parameter,
                    splitOn: "ReactId, AttachmentId");
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
                          c.Id, c.Content, c.IsResponse, c.ReactCount, c.UpdatedAt, c.UserId, 
                          ud.FirstName, ud.LastName , ud.Avatar,
                          urc.ReactTypeId AS ReactId, urc.UserId as ReactUserId, urc.CreatedAt, 
                          r.TypeName,
                          ur.FirstName AS ReactFirstName, ur.LastName AS ReactLastName, ur.Avatar AS ReactAvatar,
                          a.Id AS AttachmentId, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN UserDetails ud ON c.UserId = ud.UserId 
                          LEFT JOIN UserReactComment urc ON c.Id = urc.CommentId
                          LEFT JOIN ReactTypes r ON r.Id = urc.ReactTypeId
                          LEFT JOIN UserDetails ur ON urc.UserId = ur.UserId
                          LEFT JOIN CommentResponse cmr ON c.Id = cmr.ResponseId
                          LEFT JOIN
                            CommentAttachment ca ON ca.CommentId = c.Id
                          LEFT JOIN
                            Attachments a ON ca.AttachmentId = a.Id
                          Where c.IsDeleted = 0 AND c.IsResponse = 1 AND cmr.CommentId = @Id";
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
                            if (react != null && !commentEntry.Reacts.Any((item) => item.ReactUserId == react.ReactUserId))
                            {
                                commentEntry.Reacts.Add(react);
                            }
                            if (attachment != null && !commentEntry.Attachments.Any((item) => item.AttachmentId == attachment.AttachmentId))
                            {
                                commentEntry.Attachments.Add(attachment);
                            }
                            return commentEntry;
                        },
                        parameter,
                        splitOn: "ReactId, AttachmentId");
                    return commentDict.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("post/{postId}")]
        public async Task<IActionResult> CreateComment(Guid postId, CreateCommentCommand comment)
        {
            var query = @"INSERT INTO Comments (Id, CreatedAt, UpdatedAt, IsDeleted, Content, IsResponse, ReactCount ,UserId, PostId)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Content, @IsResponse, 0, @UserId, @PostId)";
            var queryAttachment = @"INSERT INTO Attachments (Id, Media, Description)
                                    OUTPUT Inserted.Id
                                    VALUES
                                    (NEWID(), @Media, @Description)";
            var queryCommentAttachment = @"INSERT INTO CommentAttachment (CommentId, AttachmentId)
                                        VALUES
                                        (@CommentId, @AttachmentId)";
            var queryResponse = @"INSERT INTO CommentResponse (CommentId, ResponseId)
                                  VALUES
                                  (@CommentId, @ResponseId)";
            var parameters = new DynamicParameters();
            parameters.Add("Content", comment.Content, DbType.String);
            parameters.Add("UserId", comment.UserId, DbType.Guid);
            parameters.Add("PostId", postId);
            parameters.Add("PostId", postId, DbType.Guid);
            parameters.Add("IsResponse", comment.IsResponse);
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
                                parameters.Add("CommentId", result);
                                parameters.Add("AttachmentId", attachmentResult);
                                await connection.ExecuteAsync(queryCommentAttachment, parameters, transaction);
                            }
                        }
                        if (comment.IsResponse == true)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("CommentId", comment.CommentId);
                            parameters.Add("ResponseId", result);
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
            var query = "UPDATE Comments SET Content = @Content, UpdatedAt = GETDATE() WHERE Id = @Id AND UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("Content", comment.Content, DbType.String);
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
            var query = "Update Comments SET isDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }
    }
}

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
            var query = @"SELECT c.Id, c.Content, c.IsReponse, c.ReactCount, c.UpdatedAt, c.UserId, ud.FirstName, ud.LastName ud.Avatar
                          FROM Comments c
                          INNER JOIN UserDetails ud ON c.UserId = ud.UserId 
                          Where c.PostId = @Id AND c.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", postId);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<CommentQuery>(query, parameter);

                return result;
            }
        }

        [HttpGet("detail/post/{postId}")]
        public async Task<IEnumerable<CommentDetailQuery>> GetCommentDetailsByPostID(Guid postId)
        {
            var query = @"SELECT 
                          c.Id, c.Content, c.IsReponse, c.ReactCount, c.UpdatedAt, c.UserId, 
                          ud.FirstName, ud.LastName ud.Avatar,
                          urc.ReactTypeId AS ReactId, urc.UserId as ReactUserId, urc.CreatedAt, 
                          r.TypeName,
                          ur.FirstName AS ReactFirstName, ur.LastName AS ReactLastName, ur.Avatar AS ReactAvatar
                          FROM Comments c
                          INNER JOIN UserDetails ud ON c.UserId = ud.UserId 
                          LEFT JOIN UserReactComment urc ON c.Id = urc.CommentId
                          LEFT JOIN React r ON r.Id = urc.ReactTypeId
                          LEFT JOIN UserDetails ur ON urc.UserId = ur.UserId
                          Where c.PostId = @Id AND c.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", postId);
            var commentResult = new CommentDetailQuery();
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<CommentDetailQuery, CommentReactQuery, CommentDetailQuery>(query,
                    map: (comment, react) =>
                    {
                        commentResult = comment;
                        if (react != null && !commentResult.Reacts.Any((item) => item.ReactUserId == react.ReactUserId))
                        {
                            commentResult.Reacts.Add(react);
                        }
                        return commentResult;
                    },
                    parameter,
                    splitOn: "ReactUserId");

                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<CommentDetailQuery> GetCommentByID(Guid id)
        {
            var query = @"SELECT 
                          c.Id, c.Content, c.IsReponse, c.ReactCount, c.UpdatedAt, c.UserId, 
                          ud.FirstName, ud.LastName ud.Avatar,
                          urc.ReactTypeId AS ReactId, urc.UserId as ReactUserId, urc.CreatedAt, 
                          r.TypeName,
                          ur.FirstName AS ReactFirstName, ur.LastName AS ReactLastName, ur.Avatar AS ReactAvatar
                          FROM Comments c
                          INNER JOIN UserDetails ud ON c.UserId = ud.UserId 
                          LEFT JOIN UserReactComment urc ON c.Id = urc.CommentId
                          LEFT JOIN React r ON r.Id = urc.ReactTypeId
                          LEFT JOIN UserDetails ur ON urc.UserId = ur.UserId
                          Where c.Id = @Id AND c.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var commentResult = new CommentDetailQuery();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<CommentDetailQuery, CommentReactQuery, CommentDetailQuery>(
                    query,
                    map: (comment, react) =>
                    {
                        if (comment != null)
                        {
                            commentResult = comment;
                        }

                        if (react != null && !commentResult.Reacts.Any((item) => item.ReactUserId == react.ReactUserId))
                        {
                            commentResult.Reacts.Add(react);
                        }
                        return commentResult;
                    },
                    parameter,
                    splitOn: "ReactId");
                    return commentResult;
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("{id}/reponse")]
        public async Task<CommentDetailQuery> GetCommentResponseByID(Guid id)
        {
            var query = @"SELECT 
                          c.Id, c.Content, c.IsReponse, c.ReactCount, c.UpdatedAt, c.UserId, 
                          ud.FirstName, ud.LastName ud.Avatar,
                          urc.ReactTypeId AS ReactId, urc.UserId as ReactUserId, urc.CreatedAt, 
                          r.TypeName,
                          ur.FirstName AS ReactFirstName, ur.LastName AS ReactLastName, ur.Avatar AS ReactAvatar
                          FROM Comments c
                          INNER JOIN UserDetails ud ON c.UserId = ud.UserId 
                          LEFT JOIN UserCommentReact urc ON c.Id = urc.CommentId
                          LEFT JOIN React r ON r.Id = urc.ReactTypeId
                          LEFT JOIN UserDetails ur ON urc.UserId = ur.UserId
                          LEFT JOIN CommentResponse cmr ON c.Id = cmr.ResponseId
                          Where c.Id = @Id AND c.IsDeleted = 0 AND c.IsResponse = 1 AND cmr.CommentId = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var commentResult = new CommentDetailQuery();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<CommentDetailQuery, CommentReactQuery, CommentDetailQuery>(
                    query,
                    map: (comment, react) =>
                    {
                        if (comment != null)
                        {
                            commentResult = comment;
                        }

                        if (react != null && !commentResult.Reacts.Any((item) => item.ReactUserId == react.ReactUserId))
                        {
                            commentResult.Reacts.Add(react);
                        }
                        return commentResult;
                    },
                    parameter,
                    splitOn: "ReactId");
                    return commentResult;
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("post/{postId}")]
        public async Task<IActionResult> CreateComment(Guid postId, CreateCommentCommand comment)
        {
            var query = @"INSERT INTO Comments (Id, CreatedAt, UpdatedAt, IsDeleted, Content, IsResponse, ReactCount ,UserId, PostId)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Content, 0, 0,@UserId, @PostId)";
            var parameters = new DynamicParameters();
            parameters.Add("Content", comment.Content, DbType.String);
            parameters.Add("UserId", comment.UserId, DbType.Guid);
            parameters.Add("PostId", postId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpPost("{Id}/response")]
        public async Task<IActionResult> CreateResponseComment(Guid id, CreateResponseCommentCommand comment)
        {
            var queryComment = @"INSERT INTO Comments (Id, CreatedAt, UpdatedAt, IsDeleted, Content, IsResponse, ReactCount ,UserId, PostId)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Content, 1, 0,@UserId, @PostId)";
            var queryResponse = @"INSERT INTO CommentResponse(CommentId, ResponseId)
                          VALUES
                          (@CommentId, @ResponseId)";
            var parameters = new DynamicParameters();
            parameters.Add("Content", comment.Content, DbType.String);
            parameters.Add("UserId", comment.UserId, DbType.Guid);
            parameters.Add("PostId", comment.PostId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var result = await connection.QueryAsync<Guid>(queryComment, parameters);
                    parameters = new DynamicParameters();
                    parameters.Add("CommentId", id);
                    parameters.Add("ResponseId", result);
                    await connection.ExecuteAsync(queryResponse, parameters);
                }
                return Ok();
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

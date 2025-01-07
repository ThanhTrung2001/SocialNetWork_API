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
    public class ReactTypesController : ControllerBase
    {
        private readonly DapperContext _context;

        public ReactTypesController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("post/{postId}")]
        public async Task<IEnumerable<ReactQuery>> GetByPostId(Guid postId)
        {
            var query = @"SELECT 
                 r.Id AS ReactId,
                 r.TypeName,
                 urp.UserId AS ReactUserId,
                 ud.FirstName AS ReactFirstName,
                 ud.LastName AS ReactLastName,
                 ud.Avatar AS ReactAvatar
                 FROM ReactTypes r
                 LEFT JOIN UserReactPost urp ON urp.ReactTypeId = r.Id
                 LEFT JOIN UserDetails ud ON urp.UserId = ud.UserId
                 WHERE 
                 urp.IsDeleted = 0 AND urp.PostId = @PostId";
            var parameters = new DynamicParameters();
            parameters.Add("PostId", postId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ReactQuery>(query, parameters);
                return result;
            }
        }

        [HttpGet("comment/{commentId}")]
        public async Task<IEnumerable<ReactQuery>> GetByCommentId(Guid commentId)
        {
            var query = @"SELECT 
                 r.Id AS ReactId,
                 r.TypeName,
                 urc.UserId AS ReactUserId,
                 ud.FirstName AS ReactFirstName,
                 ud.LastName AS ReactLastName,
                 ud.Avatar AS ReactAvatar
                 FROM ReactTypes r
                 LEFT JOIN UserReactComment urc ON urc.ReactTypeId = r.Id
                 LEFT JOIN UserDetails ud ON urc.UserId = ud.UserId
                 WHERE 
                 urc.IsDeleted = 0 AND urc.CommentId = @CommentId";
            var parameters = new DynamicParameters();
            parameters.Add("CommentId", commentId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ReactQuery>(query, parameters);
                return result;
            }
        }

        [HttpGet("message/{messageId}")]
        public async Task<IEnumerable<ReactQuery>> GetByMessageId(Guid messageId)
        {
            var query = @"SELECT 
                 r.Id AS ReactId,
                 r.TypeName,
                 urm.UserId AS ReactUserId,
                 ud.FirstName AS ReactFirstName,
                 ud.LastName AS ReactLastName,
                 ud.Avatar AS ReactAvatar
                 FROM ReactTypes r
                 LEFT JOIN UserReactMessage urm ON urm.ReactTypeId = r.Id
                 LEFT JOIN UserDetails ud ON urm.UserId = ud.UserId
                 WHERE 
                 urm.IsDeleted = 0 AND urm.MessageId = @MessageId";
            var parameters = new DynamicParameters();
            parameters.Add("MessageId", messageId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ReactQuery>(query, parameters);
                return result;
            }
        }

        [HttpPost("post")]
        public async Task<IActionResult> CreateByPost(CreatePostReactCommand react)
        {
            var query = @"INSERT INTO UserReactPost (UserId, PostId, ReactTypeId, CreatedAt, UpdatedAt, IsDeleted)
                        VALUES 
                        (@UserId, @PostId, @ReactTypeId, GETDATE(), GETDATE(), 0)";

            var parameter = new DynamicParameters();
            parameter.Add("UserId", react.UserId, DbType.Guid);
            parameter.Add("PostId", react.PostId);
            parameter.Add("ReactTypeId", react.ReactType, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }

        [HttpPost("comment")]
        public async Task<IActionResult> CreateByComment(CreateCommentReactCommand react)
        {
            var query = @"INSERT INTO UserReactComment (UserId, CommentId, ReactTypeId, CreatedAt, UpdatedAt, IsDeleted)
                        VALUES 
                        (@UserId, @CommentId, @ReactTypeId, GETDATE(), GETDATE(), 0)";

            var parameter = new DynamicParameters();
            parameter.Add("UserId", react.UserId, DbType.Guid);
            parameter.Add("CommentId", react.CommentId);
            parameter.Add("ReactTypeId", react.ReactType, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }

        [HttpPost("message")]
        public async Task<IActionResult> CreateByMessage(CreateMessageReactCommand react)
        {
            var query = @"INSERT INTO UserReactMessage (UserId, MessageId, ReactTypeId, CreatedAt, UpdatedAt, IsDeleted)
                        VALUES 
                        (@UserId, @MessageId, @ReactTypeId, GETDATE(), GETDATE(), 0)";

            var parameter = new DynamicParameters();
            parameter.Add("UserId", react.UserId, DbType.Guid);
            parameter.Add("MessageId", react.MessageId);
            parameter.Add("ReactTypeId", react.ReactType, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }

        [HttpPut("{id}/post")]
        public async Task<IActionResult> EditPostReact(Guid id, EditReactCommand react)
        {
            var query = @"UPDATE UserReactPost SET ReactTypeId = @ReactTypeId WHERE PostId = @Id AND UserId = @UserId";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            parameter.Add("ReactTypeId", react.ReactType, DbType.Int32);
            parameter.Add("UserId", react.UserId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }

        [HttpPut("{id}/comment")]
        public async Task<IActionResult> EditCommentReact(Guid id, EditReactCommand react)
        {
            var query = @"UPDATE UserReactComment SET ReactTypeId = @ReactTypeId WHERE CommentId = @Id AND UserId = @UserId";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            parameter.Add("ReactTypeId", react.ReactType, DbType.Int32);
            parameter.Add("UserId", react.UserId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }

        [HttpPut("{id}/message")]
        public async Task<IActionResult> EditMessageReact(Guid id, EditReactCommand react)
        {
            var query = @"UPDATE UserReactMessage SET ReactTypeId = @ReactTypeId WHERE MessageId = @Id AND UserId = @UserId";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            parameter.Add("ReactTypeId", react.ReactType, DbType.Int32);
            parameter.Add("UserId", react.UserId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }


        [HttpDelete("{id}/post")]
        public async Task<IActionResult> DeletePostReact(Guid id)
        {
            var query = "Update UserReactPost SET isDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }

        [HttpDelete("{id}/comment")]
        public async Task<IActionResult> DeleteCommentReact(Guid id)
        {
            var query = "Update UserReactComment SET isDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }

        [HttpDelete("{id}/message")]
        public async Task<IActionResult> DeleteMessageReact(Guid id)
        {
            var query = "Update UserReactMessage SET isDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id";
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

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
    //[Authorize]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ReactsController : ControllerBase
    {
        private readonly DapperContext _context;

        public ReactsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("post/{post_Id}")]
        public async Task<IActionResult> GetByPostId(Guid post_Id)
        {
            var query = @"SELECT 
                 urp.React_Type,
                 urp.User_Id AS React_UserId,
                 urp.Is_SharePost,
                 ud.FirstName AS React_FirstName,
                 ud.LastName AS React_LastName,
                 ud.Avatar AS React_Avatar
                 FROM User_React_Post urp
                 LEFT JOIN User_Details ud ON urp.User_Id = ud.User_Id
                 WHERE 
                 urp.Is_Deleted = 0 AND urp.Post_Id = @Post_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Post_Id", post_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<PostReactQuery>(query, parameters);
                    return Ok(ResponseModel<IEnumerable<PostReactQuery>>.Success(result));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<PostReactQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpGet("comment/{comment_Id}")]
        public async Task<IActionResult> GetByCommentId(Guid comment_Id)
        {
            var query = @"SELECT 
                 urc.React_Type,
                 urc.User_Id AS React_UserId,
                 ud.FirstName AS React_FirstName,
                 ud.LastName AS React_LastName,
                 ud.Avatar AS React_Avatar
                 FROM User_React_Comment urc
                 LEFT JOIN User_Details ud ON urc.User_Id = ud.User_Id
                 WHERE 
                 urc.Is_Deleted = 0 AND urc.Comment_Id = @Comment_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Comment_Id", comment_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<CommentReactQuery>(query, parameters);
                    return Ok(ResponseModel<IEnumerable<CommentReactQuery>>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<CommentReactQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpGet("message/{message_Id}")]
        public async Task<IActionResult> GetByMessage_Id(Guid message_Id)
        {
            var query = @"SELECT 

                 urm.React_Type,
                 urm.User_Id AS React_UserId,
                 ud.FirstName AS React_FirstName,
                 ud.LastName AS React_LastName,
                 ud.Avatar AS React_Avatar
                 FROM User_React_Message urm
                 LEFT JOIN User_Details ud ON urm.User_Id = ud.User_Id
                 WHERE 
                 urm.Is_Deleted = 0 AND urm.Message_Id = @Message_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Message_Id", message_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<MessageReactQuery>(query, parameters);
                    return Ok(ResponseModel<IEnumerable<MessageReactQuery>>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<MessageReactQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpPost("post")]
        public async Task<IActionResult> CreateByPost(CreatePostReactCommand react)
        {
            var query = @"INSERT INTO User_React_Post (User_Id, Post_Id, React_Type, Is_SharePost,Created_At, Updated_At, Is_Deleted)
                        VALUES 
                        (@User_Id, @Post_Id, @React_Type, @Is_SharePost ,GETDATE(), GETDATE(), 0)";
            var updatePostQuery = "UPDATE Posts SET React_Count = React_Count + 1 WHERE Id = @Post_Id";
            var updateSharePostQuery = "UPDATE Share_Posts SET React_Count = React_Count + 1 WHERE Id = @Post_Id";
            var parameter = new DynamicParameters();
            parameter.Add("User_Id", react.User_Id, DbType.Guid);
            parameter.Add("Post_Id", react.Post_Id);
            parameter.Add("React_Type", react.React_Type);
            parameter.Add("Is_SharePost", react.Is_SharePost);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameter);
                    if (react.Is_SharePost == false)
                    {
                        await connection.ExecuteAsync(updatePostQuery, parameter);
                    }
                    else
                    {
                        await connection.ExecuteAsync(updateSharePostQuery, parameter);
                    }
                    return Ok(ResponseModel<string>.Success("Success"));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpPost("comment")]
        public async Task<IActionResult> CreateByComment(CreateCommentReactCommand react)
        {
            var query = @"INSERT INTO User_React_Comment (User_Id, Comment_Id, React_Type, Created_At, Updated_At, Is_Deleted)
                        VALUES 
                        (@User_Id, @Comment_Id, @React_Type, GETDATE(), GETDATE(), 0)";
            var updateQuery = "UPDATE Comments SET React_Count = React_Count + 1 WHERE Id = @Comment_Id";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", react.User_Id, DbType.Guid);
            parameters.Add("Comment_Id", react.Comment_Id);
            parameters.Add("React_Type", react.React_Type);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    await connection.ExecuteAsync(updateQuery, parameters);
                    return Ok(ResponseModel<string>.Success("Success."));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpPost("message")]
        public async Task<IActionResult> CreateByMessage(CreateMessageReactCommand react)
        {
            var query = @"INSERT INTO User_React_Message (User_Id, Message_Id, React_Type, Created_At, Updated_At, Is_Deleted)
                        VALUES 
                        (@User_Id, @Message_Id, @React_Type,GETDATE(), GETDATE(), 0)";
            var updateQuery = "UPDATE Messages SET React_Count = React_Count + 1 WHERE Id = @Message_Id";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", react.User_Id, DbType.Guid);
            parameters.Add("Message_Id", react.Message_Id);
            parameters.Add("React_Type", react.React_Type);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    await connection.ExecuteAsync(updateQuery, parameters);
                    return Ok(ResponseModel<string>.Success("Success."));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpPut("post/{id}")]
        public async Task<IActionResult> EditPostReact(Guid id, EditReactCommand react)
        {
            var query = @"UPDATE User_React_Post SET React_Type = @React_Type WHERE Post_Id = @Id AND User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("React_Type", react.React_Type);
            parameters.Add("User_Id", react.User_Id);
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

        [HttpPut("comment/{id}")]
        public async Task<IActionResult> EditCommentReact(Guid id, EditReactCommand react)
        {
            var query = @"UPDATE User_React_Comment SET React_Type = @React_Type WHERE Comment_Id = @Id AND User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("React_Type", react.React_Type);
            parameters.Add("User_Id", react.User_Id);
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

        [HttpPut("message/{id}")]
        public async Task<IActionResult> EditMessageReact(Guid id, EditReactCommand react)
        {
            var query = @"UPDATE User_React_Message SET React_Type = @React_Type WHERE Message_Id = @Id AND User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("React_Type", react.React_Type);
            parameters.Add("User_Id", react.User_Id);
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


        [HttpDelete("post/{id}")]
        public async Task<IActionResult> DeletePostReact(Guid id, DeletePostReactCommand command)
        {
            var query = "Update User_React_Post SET Is_Deleted = 1, Updated_At = GETDATE() Where User_Id = @User_Id AND Post_Id = @Post_Id";
            var updatePostQuery = "UPDATE Posts SET React_Count = React_Count - 1 WHERE Id = @Post_Id";
            var updateSharePostQuery = "UPDATE Share_Posts SET React_Count = React_Count - 1 WHERE Id = @Post_Id";
            var parameter = new DynamicParameters();
            parameter.Add("User_Id", command.User_Id);
            parameter.Add("Post_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.ExecuteAsync(query, parameter);
                    if (command.Is_SharePost == false)
                    {
                        await connection.ExecuteAsync(updatePostQuery, parameter);
                    }
                    else
                    {
                        await connection.ExecuteAsync(updateSharePostQuery, parameter);
                    }
                    return Ok(ResponseModel<string>.Success("Success."));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpDelete("comment/{id}")]
        public async Task<IActionResult> DeleteCommentReact(Guid id, DeleteReactCommand command)
        {
            var query = "Update User_React_Comment SET Is_Deleted = 1, Updated_At = GETDATE() Where User_Id = @User_Id AND Post_Id = @Post_Id";
            var updateQuery = "UPDATE Comments SET React_Count = React_Count - 1 WHERE Id = @Comment_Id";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Comment_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    await connection.ExecuteAsync(updateQuery, parameters);
                    return Ok(ResponseModel<string>.Success("Success."));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpDelete("message/{id}")]
        public async Task<IActionResult> DeleteMessageReact(Guid id, DeleteReactCommand command)
        {
            var query = "Update User_React_Message SET Is_Deleted = 1, Updated_At = GETDATE() Where User_Id = @User_Id AND Post_Id = @Post_Id";
            var updateQuery = "UPDATE Messages SET React_Count = React_Count - 1 WHERE Id = @Message_Id";

            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Message_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    await connection.ExecuteAsync(updateQuery, parameters);
                    return Ok(ResponseModel<string>.Success("Success."));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

    }
}

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
    public class ReactsController : ControllerBase
    {
        private readonly DapperContext _context;

        public ReactsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("{postId}")]
        public async Task<IEnumerable<PostReactQuery>> Get(Guid postId)
        {
            var query = @"SELECT 
                 r.Id AS ReactId,
                 r.ReactType,
                 ur.Id AS ReactUserId,
                 ur.UserName AS ReactUserName,
                 ur.AvatarUrl AS ReactUserAvatar
                 FROM Reacts r
                 JOIN Users ur ON r.UserId = ur.Id
                 WHERE 
                 r.IsDeleted = 0 AND r.PostId = @Id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<PostReactQuery>(query, new { Id = postId });
                return result;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewReact react)
        {
            var query = @"INSERT INTO Reacts (Id, CreatedAt, UpdatedAt, IsDeleted, ReactType, UserId, PostId)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @ReactType, @UserId, @PostId)";
            var parameter = new DynamicParameters();
            parameter.Add("ReactType", react.ReactType, DbType.Int32);
            parameter.Add("UserId", react.UserId, DbType.Guid);
            parameter.Add("PostId", react.PostId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, NewReact react)
        {
            var query = @"UPDATE Reacts SET ReactType = @ReactType WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("ReactType", react.ReactType, DbType.Int32);
            parameter.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update Reacts SET isDeleted = 1 WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }

    }
}

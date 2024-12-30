using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactsController : ControllerBase
    {
        private readonly DapperContext _context;

        public ReactsController(DapperContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewReact react)
        {
            var query = @"INSERT INTO Comments (Id, CreatedAt, UpdatedAt, IsDeleted, ReacType, UserId, PostId)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @ReactType @UserId, @PostId)";
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
            var query = @"UPDATE Comments SET ReactType = @ReactType WHERE Id = @Id";
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

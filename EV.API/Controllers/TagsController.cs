using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Models;
using EnVietSocialNetWorkAPI.Models.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly DapperContext _context;
        public TagsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var query = "SElECT Id, Tag_Name FROM Tags";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<TagQuery>(query);
                    return Ok(ResponseModel<IEnumerable<TagQuery>>.Success(result));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<TagQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTagCommand command)
        {
            var query = @"INSERT INTO Tags (Tag_Name)
                          VALUES
                          (@Tag_Name);";
            var parameter = new DynamicParameters();
            parameter.Add("Tag_Name", command.Tag_Name);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, CreateTagCommand command)
        {
            var query = "UPDATE Tags SET Tag_Name = @Tag_Name WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Tag_Name", command.Tag_Name);
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
        public async Task<IActionResult> Delete(int id)
        {
            var query = @"DELETE FROM Tags WHERE Id = @Id";
            var queryJunction = @"DELETE FROM Notification_Tag WHERE Tag_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(queryJunction, parameter, transaction);
                        await connection.ExecuteAsync(query, parameter, transaction);
                        transaction.Commit();
                        return Ok(ResponseModel<string>.Success("Successful."));
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

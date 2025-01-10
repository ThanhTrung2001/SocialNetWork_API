using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Models.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class JoinGroupRequestsController : ControllerBase
    {
        private readonly DapperContext _context;
        public JoinGroupRequestsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("group/{group_Id}")]
        public async Task<IEnumerable<RequestJoinGroupQuery>> GetByGroupId(Guid group_Id)
        {
            var query = @"SELECT 
                            User_Id, Group_Id, Status, Is_Admin_Request, Admin_Id ,Created_At, Updated_At, Is_Deleted
                          FROM User_Request_Group
                          WHERE Group_Id = @group_Id AND Is_Deleted = 0;";
            var parameter = new DynamicParameters();
            parameter.Add("Group_Id", group_Id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<RequestJoinGroupQuery>(query, parameter);
                return result;
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(RequestJoinGroupCommand command)
        {
            var query = @"INSERT INTO User_Request_Group (User_Id, Group_Id, Status, Is_Admin_Request, Admin_Id ,Created_At, Updated_At, Is_Deleted) 
                          VALUES
                          (@User_Id, @Group_Id, 'Pending', 0, null ,GETDATE(), GETDATE(), 0)";
            var parameter = new DynamicParameters();
            parameter.Add("User_Id", command.User_Id);
            parameter.Add("Group_Id", command.Group_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameter);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        [HttpPost("admin")]
        public async Task<IActionResult> AdminRecommendUser(AdminRecommendCommand command)
        {
            var query = @"INSERT INTO User_Request_Group (User_Id, Group_Id, Status, Is_Admin_Request, Admin_Id ,Created_At, Updated_At, Is_Deleted) 
                          VALUES
                          (@User_Id, @Group_Id, 'Pending', 1, @Admin_Id ,GETDATE(), GETDATE(), 0)";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Group_Id", command.Group_Id);
            parameters.Add("Admin_Id", command.Admin_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPut]
        public async Task<IActionResult> ModifyRequest(ModifyRequestJoinCommand command)
        {
            var query = @"UPDATE User_Request_Group
                          SET Status = @Status, Updated_At = GETDATE()
                          WHERE User_Id = @User_Id AND Group_Id = @Group_Id AND (Status NOT LIKE 'Cancel' OR Status NOT LIKE 'Reject' )";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Group_Id", command.Group_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRequest(RequestJoinGroupCommand command)
        {
            var query = "UPDATE User_Request_Group SET Is_Deleted = 1, Updated_At = GETDATE() WHERE User_Id = @User_Id AND Group_Id = @Group_Id;";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Group_Id", command.Group_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}

using Dapper;
using EnVietSocialNetWorkAPI.Auth.Services;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [AllowAnonymous]
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly JWTHelper _helper;
        public OrganizationsController(DapperContext context, JWTHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        [HttpGet]
        public async Task<IEnumerable<OrganizeNodeQuery>> Get()
        {
            var query = "SELECT * FROM Organizations;";
            using (var connection = _context.CreateConnection())
            {
                // Fetch all nodes from the database
                var result = await connection.QueryAsync<OrganizeNodeQuery>(query);
                List<OrganizeNodeQuery> hierarchy = _helper.BuildHierarchy(result.ToList());
                return hierarchy;

            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganizeNodeCommand node)
        {
            var query = @"INSERT INTO Organizations (Id, Name, Description, Department, Email, Phone_Number, Address, City, Country, Level, Parent_Id, Employee_Count)
                          VALUES
                          (NEWID(), @Name, @Description,  @Department, @Email, @Phone_Number, @Address, @City, @Country, @Level, @Parent_Id, 0)";
            var parameter = new DynamicParameters();
            parameter.Add("Name", node.Name);
            parameter.Add("Description", node.Description);
            parameter.Add("Department", node.Department);
            parameter.Add("Email", node.Email);
            parameter.Add("Phone_Number", node.Phone_Number);
            parameter.Add("Address", node.Address);
            parameter.Add("City", node.City);
            parameter.Add("Country", node.Country);
            parameter.Add("Level", node.Level);
            parameter.Add("Parent_Id", node.Parent_Id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, CreateOrganizeNodeCommand node)
        {
            var query = @"UPDATE Organizations SET Name = @Name, Department = @Department, Email = @Email, Phone_Number = @Phone_Number, Address = @Address, City = @City, Country = @Country ,Level = @Level, ParentId = @ParentId WHERE ID = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            parameter.Add("Name", node.Name);
            parameter.Add("Description", node.Description);
            parameter.Add("Department", node.Department);
            parameter.Add("Email", node.Email);
            parameter.Add("Phone_Number", node.Phone_Number);
            parameter.Add("Address", node.Address);
            parameter.Add("City", node.City);
            parameter.Add("Country", node.Country);
            parameter.Add("Level", node.Level);
            parameter.Add("ParentId", node.Parent_Id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "UPDATE Organizations SET Is_Deleted = 1 WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpGet("{id}/users")]
        public async Task<IEnumerable<OrganizationUserQuery>> GetUsersByID(Guid id)
        {
            var query = @"SELECT 
                            o.Department,
                            uo.Organization_Role,
                            uo.User_Id,
                            ud.FirstName,
                            ud.LastName,
                            ud.Avatar
                          FROM Organizations o
                          INNER JOIN 
                            User_Organization uo ON o.Id = uo.Node_Id
                          LEFT JOIN
                            User_Details ud ON uo.User_Id = ud.User_Id
                          WHERE
                            uo.Is_Deleted = 0 AND o.Id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<OrganizationUserQuery>(query, parameters);
                return result;
            }
        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> CreateUserInOrganization(Guid id, CreateOrganizationUserCommand command)
        {
            //Check if existed
            var existQuery = @"SELECT COUNT(1) FROM User_Organization
                   WHERE User_Id = @User_Id AND Node_Id = @Organization_Id;";

            var query = @"INSERT INTO User_Organization (User_Id, Node_Id, Created_At, Updated_At, Is_Deleted, Organization_Role)
                          VALUES 
                          (@User_Id, @Organization_Id, GETDATE(), GETDATE(), 0, @Organization_Role)";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Organization_Id", id);
            parameters.Add("Organization_Role", command.Organization_Role);
            using (var connection = _context.CreateConnection())
            {
                bool existed = await connection.ExecuteScalarAsync<bool>(existQuery, parameters);
                if (existed)
                {
                    return BadRequest("Existed Connection between User and Organization");
                }
                else
                {
                    await connection.ExecuteAsync(query, parameters);
                    return Ok();
                }
            }
        }

        [HttpPut("{id}/users")]
        public async Task<IActionResult> EditUserInOrganization(Guid id, EditOrganizationUserCommand command)
        {
            var query = "UPDATE User_Organization SET Organization_Role = @Organization_Role WHERE User_Id = @User_Id AND Node_Id = @Organization_Id;";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Organization_Id", id);
            parameters.Add("Organization_Role", command.Organization_Role);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpDelete("{id}/users")]
        public async Task<IActionResult> DeleteUserInOrganization(Guid id, DeleteOrganizationUserCommand command)
        {
            var query = "UPDATE User_Organization SET Is_Deleted = 1 WHERE User_Id = @User_Id AND Node_Id = @Organization_Id";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Organization_Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

    }
}

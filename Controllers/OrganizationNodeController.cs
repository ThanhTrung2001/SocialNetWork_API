using Dapper;
using EnVietSocialNetWorkAPI.Auth.Services;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationNodeController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly JWTHelper _helper;
        public OrganizationNodeController(DapperContext context, JWTHelper helper)
        {
            _context = context;
            _helper = helper;
        }

        [HttpGet]
        public async Task<IEnumerable<OrganizeNode>> Get()
        {
            var query = "SELECT * FROM OrganizeNodes WHERE IsDeleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                // Fetch all nodes from the database
                var result = await connection.QueryAsync<OrganizeNode>(query);
                List<OrganizeNode> hierarchy = _helper.BuildHierarchy(result.ToList());
                return hierarchy;

            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewOrganizeNode node)
        {
            var query = @"INSERT INTO OrganizeNodes (Id, Name, Description, FileType, Level, ParentId, IsDeleted)
                          VALUES
                          (NEWID(), @Name, @Description, @FileType, @Level, @ParentId, 0)";
            var parameter = new DynamicParameters();
            parameter.Add("Name", node.Name);
            parameter.Add("Description", node.Description);
            parameter.Add("FileType", node.FileType);
            parameter.Add("Level", node.Level);
            parameter.Add("ParentId", node.ParentId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, NewOrganizeNode node)
        {
            var query = @"UPDATE OrganizeNodes SET Name = @Name, FileType = @FileType, Level = @Level, ParentId = @ParentId WHERE ID = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            parameter.Add("Name", node.Name);
            parameter.Add("Description", node.Description);
            parameter.Add("FileType", node.FileType);
            parameter.Add("Level", node.Level);
            parameter.Add("ParentId", node.ParentId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok(result);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "UPDATE OrganizeNodes SET isDeleted = 1 WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }


    }
}

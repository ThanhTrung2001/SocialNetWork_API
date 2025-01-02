using Dapper;
using EnVietSocialNetWorkAPI.Auth.Services;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [AllowAnonymous]
    //[Authorize]
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
            var query = "SELECT * FROM OrganizeNodes";
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
            var query = @"INSERT INTO OrganizeNodes (Id, Name, Description, FileType, Level, ParentId)
                          VALUES
                          (NEWID(), @Name, @Description, @FileType, @Level, @ParentId)";
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


    }
}

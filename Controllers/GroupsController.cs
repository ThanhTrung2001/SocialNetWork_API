using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models;
using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;
using EnVietSocialNetWorkAPI.Entities.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly DapperContext _context;

        public GroupsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<GroupQuery>> Get()
        {
            var query = @"SELECT g.Id, g.GroupName, u.Id as UserId, u.UserName, u.AvatarUrl, u.Email, ug.RoleId 
                          FROM Groups g
                          INNER JOIN UserGroup ug ON g.Id = ug.GroupId
                          LEFT JOIN Users u ON ug.UserId = u.Id
                          WHERE g.IsDeleted = 0;";

            try
            {
                var postDict = new Dictionary<Guid, GroupQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<GroupQuery, UserGroupQuery, GroupQuery>(
                    query,
                    map: (group, user) =>
                    {
                        if (!postDict.TryGetValue(group.Id, out var groupEntry))
                        {
                            groupEntry = group;
                            postDict.Add(group.Id, groupEntry);
                        }

                        if (user != null && !groupEntry.Users.Any((item) => item.Id == user.Id))
                        {
                            groupEntry.Users.Add(user);
                        }
                        return groupEntry;
                    },

                    splitOn: "UserId");
                    return postDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("search")]
        public async Task<IEnumerable<Group>> GetBySearch([FromQuery] string name)
        {
            var query = @"SELECT * FROM Groups WHERE GroupName LIKE @name; AND IsDeleted = 0;";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Group>(query, new { name = $"%{name}%" });
                return result;
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IEnumerable<Group>> GetUserJoinedGroups(Guid userId)
        {
            var query = "SELECT * FROM Groups g JOIN UserGroup ug WHERE g.Id = ug.GroupId AND ug.UserId = @UserId AND g.IsDeleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Group>(query, new { UserId = userId });
                return result;
            }
        }

        [HttpGet("{id}/users")]
        public async Task<IEnumerable<User>> GetUsersInGroup(Guid id)
        {
            var query = "SELECT * FROM Users u JOIN UserGroup ug WHERE u.Id = ug.Id AND ug.GroupId = @Id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<User>(query, new { Id = id });
                return result;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewGroup group)
        {
            var groupId = Guid.NewGuid();
            var query = @"INSERT INTO Posts (Id, CreatedAt, UpdatedAt, IsDeleted, GroupName, WallpaperURL)
                        VALUES 
                        (@Id, GETDATE(), GETDATE(), 0, @GroupName, @WallpaperURL);";
            var queryUser = @"INSERT INTO UserGroup (UserId, GroupId, RoleId, JoinedAt
                              VALUES      
                              (@UserId, @Id, 2, GETDATE());";
            var parameter = new DynamicParameters();
            parameter.Add("Id", groupId, DbType.Guid);
            parameter.Add("GroupName", group.GroupName, DbType.String);
            parameter.Add("WallpaperURL", group.WallapperURL, DbType.String);
            parameter.Add("UserId", group.Users[0]);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameter);
                    await connection.ExecuteAsync(queryUser, parameter);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> AddUsersToGroup(Guid id, NewGroup group)
        {
            var query = @"INSERT INTO UserGroup (UserId, GroupId, RoleId, JoinedAt
                              VALUES      
                              (@UserId, @Id, 2, GETDATE());";
            using (var connection = _context.CreateConnection())
            {
                foreach (var item in group.Users)
                {
                    await connection.ExecuteAsync(query, new { UserId = item, Id = id });
                }
                return Ok();
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "UPDATE Groups SET isDeleted = 1 WHERE Id = @Id;";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }

        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUserInGroup(Guid id)
        {
            var query = "UPDATE UserGroup SET isDeleted = 1 WHERE Id = @Id;";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }

    }
}

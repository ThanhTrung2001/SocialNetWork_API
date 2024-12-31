using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models;
using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;
using EnVietSocialNetWorkAPI.Entities.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
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
        public async Task<IEnumerable<Group>> GetGroupsUserJoined(Guid userId)
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

        [HttpGet("{id}/post")]
        public async Task<IEnumerable<PostBasicQuery>> GetPostsInGroup(Guid id)
        {
            var query = @"SELECT 
                         p.Id AS PostId,
                         p.Content AS PostContent,
                         p.PostType,
                         p.CreatedAt,
                         p.PostDestination,
                         u.Id AS UserId,
                         u.UserName,
                         u.Email,
                         u.AvatarUrl,
                         m.URL AS MediaUrl,
                         c.Id AS CommentId,
                         c.Content AS CommentContent,
                         c.MediaURL AS CommentMediaUrl,
                         c.CreatedAt AS CommentCreatedAt,
                         c.UserId AS CommentUserId,
                         uc.UserName AS CommentUserName,
                         uc.AvatarUrl AS CommentUserAvatarUrl,
                         r.Id AS ReactId,
                         r.ReactType,
                         ur.UserId AS ReactUserId,
                         ur.UserName AS ReactUserName,
                         ur.AvatarUrl AS ReactUserAvatar
                     FROM 
                         Posts p
                     INNER JOIN 
                         Users u ON p.OwnerId = u.Id
                     LEFT JOIN
                         MediaItems m ON p.Id = m.PostId
                     LEFT JOIN
                         Comments c ON p.Id = c.PostId
                     LEFT JOIN
                         Users uc ON c.UserId = uc.Id
                     INNER JOIN 
                         Reacts r ON p.Id = r.PostId
                     INNER JOIN
                         Users ur ON r.UserId = ur.Id 
                     WHERE 
                         p.IsDeleted = 0 AND p.PostType LIKE 'group' AND p.PostDestination = @Id;";

            try
            {
                var postDict = new Dictionary<Guid, PostBasicQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostBasicQuery, string, PostCommentQuery, PostReactQuery, PostBasicQuery>(
                    query,
                    map: (post, mediaUrl, comment, react) =>
                    {
                        if (!postDict.TryGetValue(post.PostId, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.PostId, postEntry);
                        }

                        if (!string.IsNullOrEmpty(mediaUrl) && !postEntry.MediaUrls.Any((item) => item == mediaUrl))
                        {
                            postEntry.MediaUrls.Add(mediaUrl);
                        }

                        if (comment != null && !postEntry.Comments.Any((item) => item.CommentId == comment.CommentId))
                        {
                            postEntry.Comments.Add(comment);
                        }
                        if (react != null && !postEntry.Reacts.Any((item) => item.ReactId == react.ReactId))
                        {
                            postEntry.Reacts.Add(react);
                        }
                        return postEntry;
                    },
                    new { Id = id },
                    splitOn: "MediaUrl,CommentId, ReactId");
                    return postDict.Values.ToList();
                }
            }
            catch
            {
                throw;
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

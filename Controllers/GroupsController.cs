using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Models.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
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
            var query = @"SELECT g.Id, g.GroupName, g.Avatar, g.Wallpaper, ud.Id as UserId, ud.FirstName, ud.LastName, ud.Avatar as UserAvatar, ug.Role, ug.JoinedAt
                          FROM Groups g
                          LEFT JOIN UserGroup ug ON g.Id = ug.GroupId
                          LEFT JOIN UserDetails ud ON ug.UserId = ud.UserId
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

                        if (user != null && !groupEntry.Users.Any((item) => item.UserId == user.UserId))
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
        public async Task<IEnumerable<GroupQuery>> GetBySearch([FromQuery] string name)
        {
            var query = @"SELECT g.Id, g.GroupName, g.Avatar, g.Wallpaper, ud.Id as UserId, ud.FirstName, ud.LastName, ud.Avatar as UserAvatar, ug.Role, ug.JoinedAt
                          FROM Groups g
                          LEFT JOIN UserGroup ug ON g.Id = ug.GroupId
                          LEFT JOIN UserDetails ud ON ug.UserId = ud.UserId
                          WHERE g.GroupName LIKE @Name AND g.IsDeleted = 0;";
            var parameter = new DynamicParameters();
            parameter.Add("Name", $"%{name}%");
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

                        if (user != null && !groupEntry.Users.Any((item) => item.UserId == user.UserId))
                        {
                            groupEntry.Users.Add(user);
                        }
                        return groupEntry;
                    },
                    parameter,
                    splitOn: "UserId");
                    return postDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IEnumerable<GroupQuery>> GetGroupsUserJoined(Guid userId)
        {
            var query = @"SELECT g.Id, g.GroupName, g.Avatar, g.Wallpaper, ud.UserId, ud.FirstName, ud.LastName, ud.Avatar as UserAvatar, 
                          ug.Role, ug.JoinedAt
                          FROM Groups g
                          LEFT JOIN UserGroup ug ON g.Id = ug.GroupId
                          LEFT JOIN UserDetails ud ON ug.UserId = ud.UserId
                          WHERE ug.UserId = @UserId AND g.IsDeleted = 0;";
            var parameter = new DynamicParameters();
            parameter.Add("UserId", userId);
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

                        if (user != null && !groupEntry.Users.Any((item) => item.UserId == user.UserId))
                        {
                            groupEntry.Users.Add(user);
                        }
                        return groupEntry;
                    },
                    parameter,
                    splitOn: "UserId");
                    return postDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("{id}/users")]
        public async Task<IEnumerable<UserGroupQuery>> GetUsersInGroup(Guid id)
        {
            var query = @"SELECT ud.Id as UserId, ud.FirstName, ud.LastName, ud.Avatar as UserAvatar, ug.Role, ug.JoinedAt 
                          FROM UserDetails ud
                          LEFT JOIN UserGroup ug ON ud.UserId = ug.UserId
                          WHERE ug.GroupId = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<UserGroupQuery>(query, new { Id = id });
                return result;
            }
        }

        [HttpGet("{id}/posts")]
        public async Task<IEnumerable<PostQuery>> GetPostsInGroup(Guid id)
        {
            var query = @"
            SELECT 
                p.Id AS PostId,
                p.Content,
                p.PostTypeId,
                p.CreatedAt,
                p.InGroup,
                p.DestinationId,
                p.UserId,
                u.Email,
                ud.FirstName,
                ud.LastName,
                ud.Avatar,

                a.Id AS AttachmentId,
                a.Media,
                a.Description,

                s.Id AS SurveyId,
                s.ExpiredAt,
                s.Question,
                s.Total,

                si.Id AS SurveyItemId,
                si.OptionName AS SurveyItemName,
                si.Total AS ItemTotal,

                udv.UserId AS UserVoteId,
                udv.FirstName AS VoteFirstName,
                udv.LastName AS VoteLastName,
                udv.Avatar AS VoteAvatar,
                 
                c.Id AS CommentId,
                c.Content AS CommentContent,
                c.CreatedAt AS CommentCreatedAt,
                c.UserId AS CommentUserId,
                udc.FirstName AS CommentFirstName,
                udc.LastName AS CommentLastName,
                udc.Avatar AS CommentAvatar,

                r.Id AS ReactId,
                r.TypeName,
                udr.UserId AS ReactUserId,
                udr.FirstName AS ReactFirstName,
                udr.LastName AS ReactLastName,
                udr.Avatar AS ReactAvatar,
                udr.CreatedAt

            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.UserId = u.Id
            INNER JOIN 
                UserDetails ud ON u.Id = ud.UserId
            LEFT JOIN
                PostAttachment pa ON pa.PostId = p.Id
            LEFT JOIN
                Attachments a ON pa.AttachmentId = a.Id
            LEFT JOIN 
                Surveys s ON p.Id = s.PostId
            LEFT JOIN 
                SurveyItems si ON s.Id = si.SurveyId
            LEFT JOIN
                UserVote uv ON si.Id = uv.SurveyItemId
            LEFT JOIN 
                UserDetails udv ON udv.UserId = uv.UserId 
            LEFT JOIN
                Comments c ON p.Id = c.PostId
            LEFT JOIN
                UserDetails udc ON c.UserId = udc.UserId
            LEFT JOIN
                UserReactPost urp ON p.Id = urp.PostId
            LEFT JOIN 
                ReactTypes r ON r.Id = urp.ReactTypeId
            LEFT JOIN
                UserDetails udr ON urp.UserId = udr.UserId 
            WHERE 
                p.IsDeleted = 0 AND p.InGroup = 1 AND p.DestinationId = @Id;";

            try
            {
                var postDict = new Dictionary<Guid, PostQuery>();
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, PostQuery>(
                    query,
                    map: (post, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!postDict.TryGetValue(post.PostId, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.PostId, postEntry);
                        }

                        if (post.PostTypeId == 1 && attachment != null && !postEntry.Attachments.Any((item) => item.AttachmentId == attachment.AttachmentId))
                        {
                            postEntry.Attachments.Add(attachment);
                        }

                        if (post.PostTypeId == 2 && survey != null)
                        {
                            postEntry.Survey = survey;
                            if (surveyItem != null && !postEntry.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                postEntry.Survey.SurveyItems.Add(surveyItem);
                                var result = postEntry.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.Votes.Any((item) => item.UserVoteId == vote.UserVoteId))
                                {
                                    result.Votes.Add(vote);
                                }
                            }
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
                    parameter,
                    splitOn: "AttachmentId, SurveyId, SurveyItemId, UserVoteId, CommentId, ReactId");
                    return postDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupCommand group)
        {
            var groupId = Guid.NewGuid();
            var query = @"INSERT INTO Groups (Id, CreatedAt, UpdatedAt, IsDeleted, GroupName, Avatar, Wallpaper)
                        VALUES
                        (@Id, GETDATE(), GETDATE(), 0, @GroupName, @Avatar ,@Wallpaper);";
            var queryUser = @"INSERT INTO UserGroup (UserId, GroupId, Role, JoinedAt, IsDeleted)
                              VALUES      
                              (@UserId, @Id, 2, GETDATE(), 0);";
            var parameters = new DynamicParameters();
            parameters.Add("Id", groupId, DbType.Guid);
            parameters.Add("GroupName", group.GroupName, DbType.String);
            parameters.Add("Avatar", group.Avatar, DbType.String);
            parameters.Add("Wallpaper", group.Wallapper, DbType.String);
            parameters.Add("UserId", group.Users[0]);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    await connection.ExecuteAsync(queryUser, parameters);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> AddUsersToGroup(Guid id, AddUsersToGroupCommand group)
        {
            var query = @"INSERT INTO UserGroup (UserId, GroupId, Role, JoinedAt, IsDeleted)
                              VALUES      
                              (@UserId, @Id, 2, GETDATE(), 0);";
            using (var connection = _context.CreateConnection())
            {
                foreach (var item in group.Users)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("UserId", item);
                    parameter.Add("Id", id);
                    await connection.ExecuteAsync(query, parameter);
                }
                return Ok();
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "UPDATE Groups SET isDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }

        [HttpDelete("{id}/user")]
        public async Task<IActionResult> DeleteUserInGroup(Guid id)
        {
            var query = "UPDATE UserGroup SET isDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }

    }
}

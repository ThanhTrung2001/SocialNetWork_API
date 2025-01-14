using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
using EnVietSocialNetWorkAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[Authorize]
    [AllowAnonymous]
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
        public async Task<IActionResult> Get()
        {
            var query = @"SELECT g.Id, g.Name, g.Avatar, g.Wallpaper, ug.User_Id, ug.Role, ug.Joined_At,  ud.FirstName, ud.LastName, ud.Avatar as User_Avatar
                          FROM Groups g
                          LEFT JOIN User_Group ug ON g.Id = ug.Group_Id
                          LEFT JOIN User_Details ud ON ug.User_Id = ud.User_Id
                          WHERE g.Is_Deleted = 0;";
            try
            {
                var groupDict = new Dictionary<Guid, GroupQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<GroupQuery, UserGroupQuery, GroupQuery>(
                    query,
                    map: (group, user) =>
                    {
                        if (!groupDict.TryGetValue(group.Id, out var groupEntry))
                        {
                            groupEntry = group;
                            groupDict.Add(group.Id, groupEntry);
                        }

                        if (user != null && !groupEntry.Users.Any((item) => item.User_Id == user.User_Id))
                        {
                            groupEntry.Users.Add(user);
                        }
                        return groupEntry;
                    },

                    splitOn: "User_Id");
                    return Ok(ResponseModel<IEnumerable<GroupQuery>>.Success(groupDict.Values.ToList()));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<GroupQuery>>.Failure(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = @"SELECT g.Id, g.Name, g.Avatar, g.Wallpaper, ug.User_Id, ug.Role, ug.Joined_At,  ud.FirstName, ud.LastName, ud.Avatar as User_Avatar
                          FROM Groups g
                          LEFT JOIN User_Group ug ON g.Id = ug.Group_Id
                          LEFT JOIN User_Details ud ON ug.User_Id = ud.User_Id
                          WHERE g.Is_Deleted = 0 AND g.Id = @Id;";
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
                var groupDict = new Dictionary<Guid, GroupQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<GroupQuery, UserGroupQuery, GroupQuery>(
                    query,
                    map: (group, user) =>
                    {
                        if (!groupDict.TryGetValue(group.Id, out var groupEntry))
                        {
                            groupEntry = group;
                            groupDict.Add(group.Id, groupEntry);
                        }

                        if (user != null && !groupEntry.Users.Any((item) => item.User_Id == user.User_Id))
                        {
                            groupEntry.Users.Add(user);
                        }
                        return groupEntry;
                    },
                    parameter,
                    splitOn: "User_Id");
                    return Ok(ResponseModel<GroupQuery>.Success(groupDict.Values.ToList()[0] ?? null));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<GroupQuery>.Failure(ex.Message));
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetBySearch([FromQuery] string name)
        {
            var query = @"SELECT g.Id, g.Name, g.Avatar, g.Wallpaper, ug.User_Id, ud.FirstName, ud.LastName, ud.Avatar as User_Avatar, ug.Role, ug.Joined_At
                          FROM Groups g
                          LEFT JOIN User_Group ug ON g.Id = ug.Group_Id
                          LEFT JOIN User_Details ud ON ug.User_Id = ud.User_Id
                          WHERE g.Name LIKE @Name AND g.Is_Deleted = 0;";
            var parameter = new DynamicParameters();
            parameter.Add("Name", $"%{name}%");
            try
            {
                var groupDict = new Dictionary<Guid, GroupQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<GroupQuery, UserGroupQuery, GroupQuery>(
                    query,
                    map: (group, user) =>
                    {
                        if (!groupDict.TryGetValue(group.Id, out var groupEntry))
                        {
                            groupEntry = group;
                            groupDict.Add(group.Id, groupEntry);
                        }

                        if (user != null && !groupEntry.Users.Any((item) => item.User_Id == user.User_Id))
                        {
                            groupEntry.Users.Add(user);
                        }
                        return groupEntry;
                    },
                    parameter,
                    splitOn: "User_Id");
                    return Ok(ResponseModel<IEnumerable<GroupQuery>>.Success(groupDict.Values.ToList()));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<GroupQuery>>.Failure(ex.Message));
            }
        }

        [HttpGet("user/{user_Id}")]
        public async Task<IActionResult> GetGroupsUserJoined(Guid user_Id)
        {
            var query = @"SELECT g.Id, g.Name, g.Avatar, g.Wallpaper, ud.User_Id, ud.FirstName, ud.LastName, ud.Avatar as User_Avatar, 
                          ug.Role, ug.Joined_At
                          FROM Groups g
                          LEFT JOIN User_Group ug ON g.Id = ug.Group_Id
                          LEFT JOIN User_Details ud ON ug.User_Id = ud.User_Id
                          WHERE ug.User_Id = @User_Id AND g.Is_Deleted = 0;";
            var parameter = new DynamicParameters();
            parameter.Add("User_Id", user_Id);
            try
            {
                var groupDict = new Dictionary<Guid, GroupQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<GroupQuery, UserGroupQuery, GroupQuery>(
                    query,
                    map: (group, user) =>
                    {
                        if (!groupDict.TryGetValue(group.Id, out var groupEntry))
                        {
                            groupEntry = group;
                            groupDict.Add(group.Id, groupEntry);
                        }

                        if (user != null && !groupEntry.Users.Any((item) => item.User_Id == user.User_Id))
                        {
                            groupEntry.Users.Add(user);
                        }
                        return groupEntry;
                    },
                    parameter,
                    splitOn: "User_Id");
                    return Ok(ResponseModel<IEnumerable<GroupQuery>>.Success(groupDict.Values.ToList()));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<GroupQuery>>.Failure(ex.Message));
            }
        }

        [HttpGet("{id}/users")]
        public async Task<IEnumerable<UserGroupQuery>> GetUsersInGroup(Guid id)
        {
            var query = @"SELECT ug.User_Id, ud.FirstName, ud.LastName, ud.Avatar as User_Avatar, ug.Role, ug.Joined_At 
                          FROM User_Details ud
                          LEFT JOIN User_Group ug ON ud.User_Id = ug.User_Id
                          WHERE ug.Group_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<UserGroupQuery>(query, parameter);
                return result;
            }
        }

        [HttpGet("{id}/posts")]
        public async Task<IActionResult> GetPostsInGroup(Guid id)
        {
            try
            {
                var postDict = new Dictionary<Guid, PostQuery>();
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
                parameter.Add("In_Group", 1);
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, PostQuery>(
                    "GetPostsFilter",
                    map: (post, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!postDict.TryGetValue(post.Id, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.Id, postEntry);
                        }

                        if (post.Post_Type == "Normal" && attachment != null && !postEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            postEntry.Attachments.Add(attachment);
                        }

                        if (post.Post_Type == "Survey" && survey != null)
                        {
                            postEntry.Survey = survey;
                            if (surveyItem != null && !postEntry.Survey.SurveyItems.Any((item) => item.SurveyItem_Id == surveyItem.SurveyItem_Id))
                            {
                                postEntry.Survey.SurveyItems.Add(surveyItem);
                                var result = postEntry.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItem_Id == surveyItem.SurveyItem_Id);
                                if (vote != null && !result.Votes.Any((item) => item.Vote_UserId == vote.Vote_UserId))
                                {
                                    result.Votes.Add(vote);
                                }
                            }
                        }

                        if (comment != null && !postEntry.Comments.Any((item) => item.Comment_Id == comment.Comment_Id))
                        {
                            postEntry.Comments.Add(comment);
                        }
                        if (react != null && !postEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                        {
                            postEntry.Reacts.Add(react);
                        }
                        return postEntry;
                    },
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
                    return Ok(ResponseModel<IEnumerable<PostQuery>>.Success(postDict.Values.ToList()));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<PostQuery>>.Failure(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupCommand group)
        {
            var query = @"INSERT INTO Groups (Id, Created_At, Updated_At, Is_Deleted, Name, Avatar, Wallpaper)
                        OUTPUT Inserted.Id
                        VALUES
                        (NEWID(), GETDATE(), GETDATE(), 0, @Name, @Avatar ,@Wallpaper);";
            var queryUser = @"INSERT INTO User_Group (User_Id, Group_Id, Role, Is_Follow ,Joined_At, Updated_At, Is_Deleted)
                              VALUES      
                              (@User_Id, @Id, @Role, 1, GETDATE(), GETDATE(),0);";
            var parameters = new DynamicParameters();
            parameters.Add("Name", group.Name, DbType.String);
            parameters.Add("Avatar", group.Avatar, DbType.String);
            parameters.Add("Wallpaper", group.Wallapper, DbType.String);
            parameters.Add("User_Id", group.Users[0].User_Id);
            parameters.Add("Role", group.Users[0].Role);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QuerySingleAsync<Guid>(query, parameters);
                    parameters.Add("Id", result, DbType.Guid);
                    await connection.ExecuteAsync(queryUser, parameters);
                    return Ok(ResponseModel<Guid>.Success(result));
                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<Guid>.Failure(ex.Message));
                }
            }

        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> AddUsersToGroup(Guid id, ModifyGroupUsersCommand group)
        {
            //check if exist
            var existQuery = @"SELECT COUNT(1) FROM User_Group
                               WHERE User_Id = @User_Id AND Group_Id = @Group_Id;";

            var query = @"INSERT INTO User_Group (User_Id, Group_Id, Role, Is_Follow, Joined_At, Updated_At ,Is_Deleted)
                              VALUES      
                              (@User_Id, @group_Id, @Role, @Is_Follow ,GETDATE(), GETDATE() ,0);";
            using (var connection = _context.CreateConnection())
            {
                foreach (var item in group.Users)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("User_Id", item.User_Id);
                    parameters.Add("Group_Id", id);
                    parameters.Add("Role", item.Role);
                    parameters.Add("Is_Follow", item.Is_Follow ?? true);
                    bool existed = await connection.ExecuteScalarAsync<bool>(existQuery, parameters);
                    if (existed)
                    {
                        return BadRequest(ResponseModel<string>.Failure("Existed Connection between User and Page"));
                    }
                    else
                    {
                        await connection.ExecuteAsync(query, parameters);
                    }
                }
                return Ok(ResponseModel<string>.Success("Success"));
            }
        }

        [HttpPut("{id}/users")]
        public async Task<IActionResult> EditUserInGroup(Guid id, ModifyGroupUserCommand group)
        {
            var queryUserGroup = "UPDATE User_Group SET Is_Follow = @Is_Follow, Role = @Role WHERE Group_Id = @Id AND User_Id = @User_Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("User_Id", group.User_Id);
            parameters.Add("Role", group.Role);
            parameters.Add("Is_Follow", group.Is_Follow);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(queryUserGroup, parameters, transaction);
                        transaction.Commit();
                        return Ok(ResponseModel<string>.Success("Success."));

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<string>.Failure(ex.Message));
                    }
                }
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "UPDATE Groups SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id;";
            var queryUserGroup = "UPDATE User_Group SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Group_Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameter, transaction);
                        await connection.ExecuteAsync(queryUserGroup, parameter, transaction);
                        transaction.Commit();
                        return Ok(ResponseModel<string>.Success("Success."));

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<string>.Failure(ex.Message));
                    }
                }
            }
        }

        [HttpDelete("{id}/users")]
        public async Task<IActionResult> DeleteUserInGroup(Guid id)
        {
            var query = "UPDATE User_Group SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Group_Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Group_Id", id, DbType.Guid);
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

    }
}

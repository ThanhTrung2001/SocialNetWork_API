﻿using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
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
        public async Task<IEnumerable<GroupQuery>> Get()
        {
            var query = @"SELECT g.Id, g.Name, g.Avatar, g.Wallpaper, ug.User_Id, ug.Role, ug.Joined_At,  ud.FirstName, ud.LastName, ud.Avatar as User_Avatar
                          FROM Groups g
                          LEFT JOIN User_Group ug ON g.Id = ug.Group_Id
                          LEFT JOIN User_Details ud ON ug.User_Id = ud.User_Id
                          WHERE g.Is_Deleted = 0;";
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

                        if (user != null && !groupEntry.Users.Any((item) => item.User_Id == user.User_Id))
                        {
                            groupEntry.Users.Add(user);
                        }
                        return groupEntry;
                    },

                    splitOn: "User_Id");
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
            var query = @"SELECT g.Id, g.Name, g.Avatar, g.Wallpaper, ug.User_Id, ud.FirstName, ud.LastName, ud.Avatar as User_Avatar, ug.Role, ug.Joined_At
                          FROM Groups g
                          LEFT JOIN User_Group ug ON g.Id = ug.Group_Id
                          LEFT JOIN User_Details ud ON ug.User_Id = ud.User_Id
                          WHERE g.Name LIKE @Name AND g.Is_Deleted = 0;";
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

                        if (user != null && !groupEntry.Users.Any((item) => item.User_Id == user.User_Id))
                        {
                            groupEntry.Users.Add(user);
                        }
                        return groupEntry;
                    },
                    parameter,
                    splitOn: "User_Id");
                    return postDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("user/{user_Id}")]
        public async Task<IEnumerable<GroupQuery>> GetGroupsUserJoined(Guid user_Id)
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

                        if (user != null && !groupEntry.Users.Any((item) => item.User_Id == user.User_Id))
                        {
                            groupEntry.Users.Add(user);
                        }
                        return groupEntry;
                    },
                    parameter,
                    splitOn: "User_Id");
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
            var query = @"SELECT ug.User_Id, ud.FirstName, ud.LastName, ud.Avatar as User_Avatar, ug.Role, ug.Joined_At 
                          FROM User_Details ud
                          LEFT JOIN User_Group ug ON ud.User_Id = ug.User_Id
                          WHERE ug.Group_Id = @Id";
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
                p.Id,
                p.Content,
                p.Post_Type_Id,
                p.Created_At,
                p.In_Group,
                p.Destination_Id,
                p.User_Id,
                u.Email,
                ud.FirstName,
                ud.LastName,
                ud.Avatar,

                a.Id AS Attachment_Id,
                a.Media,
                a.Description,

                s.Id AS Survey_Id,
                s.Expired_At,
                s.Question,
                s.Survey_Type,
                s.Total_Vote,

                si.Id AS SurveyItem_Id,
                si.Option_Name AS SurveyItem_Name,
                si.Total_Vote AS Item_Total,

                udv.User_Id AS Vote_UserId,
                udv.FirstName AS Vote_FirstName,
                udv.LastName AS Vote_LastName,
                udv.Avatar AS Vote_Avatar,
                 
                c.Id AS Comment_Id,
                c.Content AS Comment_Content,
                c.Created_At AS Comment_Created_At,
                c.User_Id AS Comment_UserId,
                udc.FirstName AS Comment_FirstName,
                udc.LastName AS Comment_LastName,
                udc.Avatar AS Comment_Avatar,

                urp.React_Type,
                udr.User_Id AS React_UserId,
                udr.FirstName AS React_FirstName,
                udr.LastName AS React_LastName,
                udr.Avatar AS React_Avatar,
                udr.Created_At

            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.User_Id = u.Id
            INNER JOIN 
                User_Details ud ON u.Id = ud.User_Id
            LEFT JOIN
                Post_Attachment pa ON pa.Post_Id = p.Id
            LEFT JOIN
                Attachments a ON pa.Attachment_Id = a.Id
            LEFT JOIN 
                Surveys s ON p.Id = s.Id
            LEFT JOIN 
                Survey_Items si ON s.Id = si.Survey_Id
            LEFT JOIN
                User_SurveyItem_Vote uv ON si.Id = uv.SurveyItem_Id
            LEFT JOIN 
                User_Details udv ON udv.User_Id = uv.User_Id 
            LEFT JOIN
                Comments c ON p.Id = c.Post_Id
            LEFT JOIN
                User_Details udc ON c.User_Id = udc.User_Id
            LEFT JOIN
                User_React_Post urp ON p.Id = urp.Post_Id
            LEFT JOIN
                User_Details udr ON urp.User_Id = udr.User_Id 
            WHERE 
                p.Is_Deleted = 0 AND p.In_Group = 1 AND p.Destination_Id = @Id;";

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
                        if (!postDict.TryGetValue(post.Id, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.Id, postEntry);
                        }

                        if (post.Post_Type_Id == 1 && attachment != null && !postEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            postEntry.Attachments.Add(attachment);
                        }

                        if (post.Post_Type_Id == 2 && survey != null)
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
                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
                    return postDict.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupCommand group)
        {
            var Group_Id = Guid.NewGuid();
            var query = @"INSERT INTO Groups (Id, Created_At, Updated_At, Is_Deleted, Name, Avatar, Wallpaper)
                        VALUES
                        (@Id, GETDATE(), GETDATE(), 0, @Name, @Avatar ,@Wallpaper);";
            var queryUser = @"INSERT INTO User_Group (User_Id, Group_Id, Role, Is_Follow ,Joined_At, Updated_At, Is_Deleted)
                              VALUES      
                              (@User_Id, @Id, @Role, 1, GETDATE(), GETDATE(),0);";
            var parameters = new DynamicParameters();
            parameters.Add("Id", Group_Id, DbType.Guid);
            parameters.Add("Name", group.Name, DbType.String);
            parameters.Add("Avatar", group.Avatar, DbType.String);
            parameters.Add("Wallpaper", group.Wallapper, DbType.String);
            parameters.Add("User_Id", group.Users[0].User_Id);
            parameters.Add("Role", group.Users[0].Role);
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
        public async Task<IActionResult> AddUsersToGroup(Guid id, ModifyGroupUsersCommand group)
        {
            var query = @"INSERT INTO User_Group (User_Id, Group_Id, Role, Is_Follow, Joined_At, Updated_At ,Is_Deleted)
                              VALUES      
                              (@User_Id, @group_Id, @Role, 1 ,GETDATE(), GETDATE() ,0);";
            using (var connection = _context.CreateConnection())
            {
                foreach (var item in group.Users)
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("User_Id", item.User_Id);
                    parameter.Add("Group_Id", id);
                    parameter.Add("Role", item.Role);
                    await connection.ExecuteAsync(query, parameter);
                }
                return Ok();
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "UPDATE Groups SET Is_Deleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }

        [HttpDelete("{id}/user")]
        public async Task<IActionResult> DeleteUserIn_Group(Guid id)
        {
            var query = "UPDATE User_Group SET Is_Deleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id;";
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

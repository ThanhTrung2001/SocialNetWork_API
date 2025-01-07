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
    public class ChatGroupsController : ControllerBase
    {
        private readonly DapperContext _context;

        public ChatGroupsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<ChatGroupQuery>> Get()
        {
            var query = @"SELECT Id, ChatName, ThemeId, GroupType
                          FROM ChatGroups
                          WHERE IsDeleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ChatGroupQuery>(query);
                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<ChatGroupQuery> GetChatGroupDetail(Guid id)
        {
            var query = @"SELECT 
                          c.Id, c.ChatName, c.ThemeId, c.GroupType
                          FROM ChatGroups c
                          WHERE Id = @Id;"
                      + @"SELECT ud.UserId, ud.FirstName, ud.LastName, ud.Avatar 
                          FROM UserDetails ud
                          INNER JOIN UserChatGroup ucg ON ucg.UserId = ud.UserId
                          WHERE ucg.ChatGroupId = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                using (var multi = await connection.QueryMultipleAsync(query, parameter))
                {
                    var ChatGroup = await multi.ReadSingleOrDefaultAsync<ChatGroupQuery>();
                    if (ChatGroup != null)
                    {
                        foreach (UserChatGroupQuery user in (await multi.ReadAsync<UserChatGroupQuery>()).ToList())
                        {
                            if (user != null && !ChatGroup.Users.Any((item) => item.UserId == user.UserId))
                            {
                                ChatGroup.Users.Add(user);
                            }
                        }
                    }
                    return ChatGroup;
                }
            }
        }

        [HttpPost]
        public async Task<dynamic> CreateChatGroup(CreateChatGroupCommand ChatGroup)
        {
            if (ChatGroup.GroupType == "private")
            {
                var existResult = await GetPrivateChatGroup(ChatGroup.Users[0], ChatGroup.Users[1]);
                if (existResult != Guid.Empty)
                {
                    return BadRequest("Existed already: " + existResult);
                }
            }
            var execChatGroup = @"INSERT INTO ChatGroups (Id, CreatedAt, UpdatedAt, IsDeleted, ChatName, ThemeId, GroupType)
                        OUTPUT Inserted.Id
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @ChatName, @ThemeId, @GroupType);";
            var execUserChatGroup = @"INSERT INTO UserChatGroup (UserId, ChatGroupId) VALUES (@UserId, @ChatGroupId)";
            //
            var parameters = new DynamicParameters();
            parameters.Add("ChatName", ChatGroup.ChatName, DbType.String);
            parameters.Add("ThemeId", ChatGroup.Theme, DbType.String);
            parameters.Add("GroupType", ChatGroup.GroupType, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var resultChatGroup = await connection.QueryAsync<Guid>(execChatGroup, parameters);
                    foreach (var userId in ChatGroup.Users)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("UserId", userId);
                        parameters.Add("ChatGroupId", resultChatGroup);
                        var resultUserChatGroup = await connection.ExecuteAsync(execUserChatGroup, parameters);
                    }
                    return resultChatGroup;
                }
                catch (Exception e)
                {
                    return e;
                }
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update ChatGroups SET isDeleted = 1 WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IEnumerable<ChatGroup>> GetChatGroupsByUserID(Guid userId)
        {
            var query = @"SELECT *
                FROM ChatGroups c
                JOIN UserChatGroup ucb ON c.Id = ucb.ChatGroupId
                WHERE ucb.UserId = @UserId AND c.IsDeleted = 0;";
            //
            var parameter = new DynamicParameters();
            parameter.Add("UserId", userId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ChatGroup>(query, parameter);
                return result;
            }
        }

        [HttpGet("private/{userId1}/{userId2}")]
        public async Task<Guid> GetPrivateChatGroup(Guid userId1, Guid userId2)
        {

            var query = @"SELECT cb.Id AS ChatGroupId
                          FROM ChatGroups cb
                          INNER JOIN UserChatGroup ucb1 ON cb.Id = ucb1.ChatGroupId
                          INNER JOIN UserChatGroup ucb2 ON cb.Id = ucb2.ChatGroupId                         
                          WHERE ucb1.UserId = @User1Id AND ucb2.UserId = @User2Id AND cb.IsDeleted = 0;";
            var parameter = new DynamicParameters();
            parameter.Add("User1Id", userId1);
            parameter.Add("User2Id", userId2);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<Guid>(query, parameter);
                return result;
            }
        }
    }


}

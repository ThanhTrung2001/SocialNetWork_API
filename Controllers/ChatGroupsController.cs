using Dapper;
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
    public class Chat_GroupsController : ControllerBase
    {
        private readonly DapperContext _context;

        public Chat_GroupsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<ChatGroupQuery>> Get()
        {
            var query = @"SELECT Id, Name, Theme, Group_Type
                          FROM Chat_Groups
                          WHERE Is_Deleted = 0;";
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
                          c.Id, c.Name, c.Theme, c.Group_Type
                          FROM Chat_Groups c
                          WHERE Id = @Id;"
                      + @"SELECT ud.User_Id, ud.FirstName, ud.LastName, ud.Avatar , ucg.Role
                          FROM User_Details ud
                          INNER JOIN User_ChatGroup ucg ON ucg.User_Id = ud.User_Id
                          WHERE ucg.ChatGroup_Id = @Id;";
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
                            if (user != null && !ChatGroup.Users.Any((item) => item.User_Id == user.User_Id))
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
            if (ChatGroup.Group_Type == "private" && ChatGroup.Users.Count == 2)
            {
                var existResult = await GetPrivateChatGroup(ChatGroup.Users[0].User_Id, ChatGroup.Users[1].User_Id);
                if (existResult != Guid.Empty)
                {
                    return BadRequest("Existed already: " + existResult);
                }
            }
            var execChatGroup = @"INSERT INTO Chat_Groups (Id, Created_At, Updated_At, Is_Deleted, Name, Theme, Group_Type)
                                OUTPUT Inserted.Id
                                VALUES 
                                (NEWID(), GETDATE(), GETDATE(), 0, @Name, @Theme, @Group_Type);";
            var execUserChatGroup = @"INSERT INTO User_ChatGroup (User_Id, ChatGroup_Id, Role, Is_Not_Notification, Delay_Until) VALUES (@User_Id, @ChatGroup_Id, @Role, 0, GETDATE())";
            //
            var parameters = new DynamicParameters();
            parameters.Add("Name", ChatGroup.Name, DbType.String);
            parameters.Add("Theme", ChatGroup.Theme, DbType.String);
            parameters.Add("Group_Type", ChatGroup.Group_Type, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var resultChatGroup = await connection.QueryAsync<Guid>(execChatGroup, parameters, transaction);
                        foreach (var item in ChatGroup.Users)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("User_Id", item.User_Id);
                            parameters.Add("ChatGroup_Id", resultChatGroup);
                            parameters.Add("Role", item.Role);
                            var resultUser_ChatGroup = await connection.ExecuteAsync(execUserChatGroup, parameters, transaction);
                        }
                        transaction.Commit();
                        return resultChatGroup;
                    }
                    catch (Exception e)
                    {
                        return e;
                    }
                }
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update Chat_Groups SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> AddUsersToChatGroup(Guid id, ModifyGroupUsersCommand group)
        {
            var query = @"INSERT INTO User_ChatGroup (User_Id, ChatGroup_Id, Role, Is_Not_Notification, Delay_Until)
                              VALUES      
                              (@User_Id, @group_Id, @Role, 0, GETDATE());";
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


        [HttpGet("user/{user_Id}")]
        public async Task<IEnumerable<ChatGroupQuery>> GetChatGroupsByUserId(Guid user_Id)
        {
            var query = @"SELECT c.Id, c.Name, c.Theme, c.Group_Type
                FROM User_ChatGroup ucb
                INNER JOIN Chat_Groups c ON c.Id = ucb.ChatGroup_Id
                WHERE ucb.User_Id = @User_Id AND c.Is_Deleted = 0;";
            //
            var parameter = new DynamicParameters();
            parameter.Add("User_Id", user_Id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ChatGroupQuery>(query, parameter);
                return result;
            }
        }

        [HttpGet("private/{User_Id1}/{User_Id2}")]
        public async Task<Guid> GetPrivateChatGroup(Guid User_Id1, Guid User_Id2)
        {

            var query = @"SELECT cb.Id AS ChatGroup_Id
                          FROM Chat_Groups cb
                          INNER JOIN User_ChatGroup ucb1 ON cb.Id = ucb1.ChatGroup_Id
                          INNER JOIN User_ChatGroup ucb2 ON cb.Id = ucb2.ChatGroup_Id                         
                          WHERE ucb1.User_Id = @User1Id AND ucb2.User_Id = @User2Id AND cb.Is_Deleted = 0;";
            var parameter = new DynamicParameters();
            parameter.Add("User1Id", User_Id1);
            parameter.Add("User2Id", User_Id2);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<Guid>(query, parameter);
                return result;
            }
        }
    }
}

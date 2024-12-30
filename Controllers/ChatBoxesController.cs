using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork.Chat;
using EnVietSocialNetWorkAPI.Entities.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBoxesController : ControllerBase
    {
        private readonly DapperContext _context;

        public ChatBoxesController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<ChatBox>> Get()
        {
            var query = @"SELECT * FROM ChatBoxes
                        WHERE IsDeleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ChatBox>(query);
                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<ChatBoxQuery> GetChatBoxDetail(Guid id)
        {
            var query = @"SELECT Id, BoxName, BoxType, Theme 
                          FROM ChatBoxes
                          WHERE Id = @Id;"
                      + @"SELECT UserId FROM UserChatBox WHERE ChatBoxId = @Id;";
            using (var connection = _context.CreateConnection())
            {
                using (var multi = await connection.QueryMultipleAsync(query, new { id }))
                {
                    var chatbox = await multi.ReadSingleOrDefaultAsync<ChatBoxQuery>();
                    if (chatbox != null)
                    {
                        foreach (Guid userId in (await multi.ReadAsync<Guid>()).ToList())
                        {
                            if (userId != Guid.Empty && !chatbox.Users.Any((item) => item == userId))
                            {
                                chatbox.Users.Add(userId);
                            }
                        }
                    }
                    return chatbox;
                }
            }
        }

        [HttpPost]
        public async Task<dynamic> CreateChatBox(NewChatBox chatbox)
        {
            if (chatbox.Theme == "private")
            {
                var existResult = await GetPrivateChatBox(chatbox.Users[0], chatbox.Users[1]);
                if (existResult != Guid.Empty)
                {
                    return existResult;
                }
            }
            var execChatBox = @"INSERT INTO ChatBoxes (Id, CreatedAt, UpdatedAt, IsDeleted, BoxType, Theme, BoxName, AlterName, UserList)
                        VALUES 
                        (@ChatBoxId, GETDATE(), GETDATE(), 0, @BoxType, @Theme, @BoxName, @AlterName , @UserList);";
            var execUserChatBox = @"INSERT INTO UserChatBox (UserId, ChatBoxId) VALUES (@UserId, @ChatBoxId)";
            //
            var chatBoxId = Guid.NewGuid();
            //
            var parameters = new DynamicParameters();
            parameters.Add("ChatBoxId", chatBoxId, DbType.Guid);
            parameters.Add("BoxName", chatbox.BoxName, DbType.String);
            parameters.Add("BoxType", chatbox.BoxType, DbType.String);
            parameters.Add("Theme", chatbox.Theme, DbType.String);
            parameters.Add("AlterName", "Alter", DbType.String);
            parameters.Add("UserList", "", DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var resultChatBox = await connection.ExecuteAsync(execChatBox, parameters);
                    foreach (var userId in chatbox.Users)
                    {
                        var resultUserChatBox = await connection.ExecuteAsync(execUserChatBox, new { UserId = userId, ChatBoxId = chatBoxId });
                    }
                    return chatBoxId;
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
            var query = "Update ChatBoxes SET isDeleted = 1 WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IEnumerable<ChatBox>> GetChatBoxesBaseUserID(Guid userId)
        {
            //var query = @"
            //SELECT u.ID as UserId, u.UserName
            //FROM Users u
            //Where u.Id = @UserId;"
            //+ @"SELECT c.Id as ChatBoxId, c.BoxType
            //    FROM ChatBoxes c
            //    JOIN UserChatBox ucb ON c.Id = ucb.ChatBoxId
            //    WHERE ucb.UserId = @UserId;";
            //using (var connection = _context.CreateConnection())
            //{
            //    using (var multi = await connection.QueryMultipleAsync(query, new { UserId = id }))
            //    {
            //        var result = await multi.ReadSingleOrDefaultAsync<ChatBoxByUserIDQuery>();
            //        if (result != null)
            //            result.BoxList = (await multi.ReadAsync<ChatBoxQuery>()).ToList();
            //        return result;
            //    }
            //}
            var query = @"SELECT *
                FROM ChatBoxes c
                JOIN UserChatBox ucb ON c.Id = ucb.ChatBoxId
                WHERE ucb.UserId = @UserId AND c.IsDeleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ChatBox>(query, new { UserId = userId });
                return result;
            }
        }

        [HttpGet("private/{userId1}/{userId2}")]
        public async Task<Guid> GetPrivateChatBox(Guid userId1, Guid userId2)
        {

            var query = @"SELECT cb.Id AS ChatBoxId
              FROM ChatBoxes cb
              JOIN UserChatBox ucb1 ON cb.Id = ucb1.ChatBoxId
              JOIN UserChatBox ucb2 ON cb.Id = ucb2.ChatBoxId                         
              WHERE ucb1.UserId = @User1Id AND ucb2.UserId = @User2Id;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<Guid>(query, new { User1Id = userId1, User2Id = userId2 });
                return result;
            }
        }
    }


}

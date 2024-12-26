using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork.Chat;
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

        [HttpGet("{userId}")]
        public async Task<IEnumerable<ChatBox>> GetChatBoxBaseUserID(Guid userId)
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
                WHERE ucb.UserId = @UserId;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ChatBox>(query, new { UserId = userId });
                return result;
            }
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateChatBox(Guid userId, NewChatBox chatbox)
        {
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
            //
            var parameters2 = new DynamicParameters();
            parameters2.Add("UserId", userId, DbType.Guid);
            parameters2.Add("ChatBoxId", chatBoxId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                //var transaction = connection.BeginTransaction();
                var resultChatBox = await connection.ExecuteAsync(execChatBox, parameters);
                var resultUserChatBox = await connection.ExecuteAsync(execUserChatBox, parameters2);
                //try
                //{
                //    transaction.Commit();
                //}
                //catch (Exception e)
                //{
                //    transaction.Rollback();
                //}
                return Ok(resultChatBox);

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


    }


}

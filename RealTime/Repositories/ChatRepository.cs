using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using System.Data;

namespace EnVietSocialNetWorkAPI.RealTime.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly DapperContext _context;

        public ChatRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
        }

        public async Task CreateChatBox(List<Guid> users, NewChatBox chatBox)
        {
            if (chatBox.Theme == "private")
            {
                var existResult = await GetPrivateChatBox(chatBox.Users[0], chatBox.Users[1]);
                if (existResult != Guid.Empty)
                {
                    return;
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
            parameters.Add("BoxName", chatBox.BoxName, DbType.String);
            parameters.Add("BoxType", chatBox.BoxType, DbType.String);
            parameters.Add("Theme", chatBox.Theme, DbType.String);
            parameters.Add("AlterName", "Alter", DbType.String);
            parameters.Add("UserList", "", DbType.String);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    //ChatBox
                    var resultChatBox = await connection.ExecuteAsync(execChatBox, parameters);
                    //userChatBox
                    foreach (var userId in users)
                    {
                        var resultUserChatBox = await connection.ExecuteAsync(execUserChatBox, new { UserId = userId, ChatBoxId = chatBoxId });
                    }
                }
                catch (Exception e)
                {

                }
                return;
            }
        }

        public async Task<Guid> GetPrivateChatBox(Guid userId, Guid receiverId)
        {
            //WHERE cb.BoxType = 'private'
            var query = @"SELECT cb.Id AS ChatBoxId
                          FROM ChatBoxes cb
                          JOIN UserChatBox ucb1 ON cb.Id = ucb1.ChatBoxId
                          JOIN UserChatBox ucb2 ON cb.Id = ucb2.ChatBoxId                         
                          WHERE ucb1.UserId = @User1Id AND ucb2.UserId = @User2Id;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<Guid>(query, new { User1Id = userId, User2Id = receiverId });
                return result;
            }
        }

        public async Task AddUserToChatBox(Guid id, Guid UserId)
        {
            var query = @"INSERT INTO UserChatBox (UserId, ChatBoxId) VALUES (@UserId, @ChatBoxId)";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { UserId = id, ChatBoxId = id });
                return;
            }
        }


        public async Task DeleteChatBox(Guid id)
        {
            var query = "Update ChatBoxes SET isDeleted = 1 WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return;
            }
        }

        public async Task SaveMessage(NewMessage message, Guid chatBoxId)
        {
            var query = @"INSERT INTO Messages (Id, CreatedAt, UpdatedAt, IsDeleted, Content, UserId, ChatBoxId)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @Content, @UserId, @ChatBoxId);";
            var parameters = new DynamicParameters();
            parameters.Add("Content", message.Content, DbType.String);
            parameters.Add("UserId", message.UserId, DbType.Guid);
            parameters.Add("ChatBoxId", chatBoxId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.ExecuteAsync(query, parameters);
                    return;
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

        public async Task DeleteMessage(Guid messageId)
        {
            var query = "Update Messages SET isDeleted = 1 WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = messageId });
                return;
            }
        }

    }
}
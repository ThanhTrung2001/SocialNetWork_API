using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;
using System.Data;

namespace EV.DataAccess.Repositories
{
    public class ChatGroupRepository
    {
        private readonly DatabaseContext _context;

        public ChatGroupRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<ChatGroupQuery>>> Get()
        {
            var query = @"SELECT Id, Name, Theme, Group_Type
                          FROM Chat_Groups
                          WHERE Is_Deleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<ChatGroupQuery>(query);
                    return ResponseModel<IEnumerable<ChatGroupQuery>>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<ChatGroupQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<Guid>> GetPrivateChatGroup(Guid user1, Guid user2)
        {
            var query = @"SELECT cb.Id AS ChatGroup_Id
                          FROM Chat_Groups cb
                          INNER JOIN User_ChatGroup ucb1 ON cb.Id = ucb1.ChatGroup_Id
                          INNER JOIN User_ChatGroup ucb2 ON cb.Id = ucb2.ChatGroup_Id                         
                          WHERE ucb1.User_Id = @User1Id AND ucb2.User_Id = @User2Id AND cb.Is_Deleted = 0;";
            var parameters = new DynamicParameters();
            parameters.Add("User1Id", user1);
            parameters.Add("User2Id", user2);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QuerySingleAsync<Guid>(query, parameters);
                    return ResponseModel<Guid>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<Guid>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<ChatGroupQuery>> GetById(Guid id)
        {
            var query = @"SELECT 
                          c.Id, c.Name, c.Theme, c.Group_Type
                          FROM Chat_Groups c
                          WHERE Id = @Id AND Is_Deleted = 0;"
                     + @"SELECT ud.User_Id, ud.FirstName, ud.LastName, ud.Avatar , ucg.Role
                          FROM User_Details ud
                          INNER JOIN User_ChatGroup ucg ON ucg.User_Id = ud.User_Id
                          WHERE ucg.ChatGroup_Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {

                    using (var multi = await connection.QueryMultipleAsync(query, parameter))
                    {
                        var chatGroup = await multi.ReadSingleOrDefaultAsync<ChatGroupQuery>();
                        if (chatGroup != null)
                        {
                            foreach (UserChatGroupQuery user in (await multi.ReadAsync<UserChatGroupQuery>()).ToList())
                            {
                                if (user != null && !chatGroup.Users.Any((item) => item.User_Id == user.User_Id))
                                {
                                    chatGroup.Users.Add(user);
                                }
                            }
                        }
                        return ResponseModel<ChatGroupQuery>.Success(chatGroup);
                    }
                }
                catch (Exception ex)
                {
                    return ResponseModel<ChatGroupQuery>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<IEnumerable<ChatGroupQuery>>> GetByUserId(Guid id)
        {
            var query = @"SELECT c.Id, c.Name, c.Theme, c.Group_Type
                FROM User_ChatGroup ucb
                INNER JOIN Chat_Groups c ON c.Id = ucb.ChatGroup_Id
                WHERE ucb.User_Id = @User_Id AND c.Is_Deleted = 0;";
            //
            var parameter = new DynamicParameters();
            parameter.Add("User_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<ChatGroupQuery>(query, parameter);
                    return ResponseModel<IEnumerable<ChatGroupQuery>>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<ChatGroupQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<Guid>> Create(CreateChatGroupCommand command)
        {
            if (command.Group_Type == "Private" && command.Users.Count == 2)
            {
                var existResult = await GetPrivateChatGroup(command.Users[0].User_Id, command.Users[1].User_Id);
                if (existResult.Result != Guid.Empty)
                {
                    return ResponseModel<Guid>.Failure($"This ChatRoom is already existed! Id: {existResult}");
                }
            }
            var execChatGroup = @"INSERT INTO Chat_Groups (Id, Created_At, Updated_At, Is_Deleted, Name, Theme, Group_Type)
                                OUTPUT Inserted.Id
                                VALUES 
                                (NEWID(), GETDATE(), GETDATE(), 0, @Name, @Theme, @Group_Type);";
            var execUserChatGroup = @"INSERT INTO User_ChatGroup (User_Id, ChatGroup_Id, Role, Is_Not_Notification, Delay_Until)
                                      VALUES 
                                      (@User_Id, @ChatGroup_Id, @Role, 0, GETDATE())";
            //
            var parameters = new DynamicParameters();
            parameters.Add("Name", command.Name, DbType.String);
            parameters.Add("Theme", command.Theme, DbType.String);
            parameters.Add("Group_Type", command.Group_Type, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var resultChatGroup = await connection.QuerySingleAsync<Guid>(execChatGroup, parameters, transaction);
                        foreach (var item in command.Users)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("User_Id", item.User_Id);
                            parameters.Add("ChatGroup_Id", resultChatGroup);
                            parameters.Add("Role", item.Role);
                            await connection.ExecuteAsync(execUserChatGroup, parameters, transaction);
                        }
                        transaction.Commit();
                        return ResponseModel<Guid>.Success(resultChatGroup);
                    }
                    catch (Exception ex)
                    {
                        return ResponseModel<Guid>.Failure(ex.Message)!;
                    }
                }
            }
        }

        public async Task<ResponseModel<string>> Edit(Guid id, EditChatGroupCommand command)
        {
            var query = @"UPDATE Chat_groups SET Name = @Name, Theme = @Theme, Group_Type = @Group_Type WHERE Id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Name", command.Name);
            parameters.Add("Theme", command.Theme);
            parameters.Add("Group_Type", command.Group_Type);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Delete(Guid id)
        {
            var query = "UPDATE Chat_Groups SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id";
            var queryMessage = "UPDATE Messages SET Is_Deleted = 1, Updated_At = GETDATE() WHERE ChatGroup_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameter, transaction);
                        await connection.ExecuteAsync(queryMessage, parameter, transaction);
                        transaction.Commit();
                        return ResponseModel<string>.Success("Success.");

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ResponseModel<string>.Failure(ex.Message)!;
                    }
                }
            }
        }

        #region [User]

        public async Task<ResponseModel<IEnumerable<UserChatGroupQuery>>> GetUsersInChatGroup(Guid id)
        {
            var query = @"SELECT ud.User_Id, ud.FirstName, ud.LastName, ud.Avatar , ucg.Role
                          FROM User_Details ud
                          INNER JOIN User_ChatGroup ucg ON ucg.User_Id = ud.User_Id
                          WHERE ucg.ChatGroup_Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<UserChatGroupQuery>(query, parameter);
                    return ResponseModel<IEnumerable<UserChatGroupQuery>>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<UserChatGroupQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> AddUserToChatGroup(Guid id, AddUsersToChatGroupCommand command)
        {
            var query = @"INSERT INTO User_ChatGroup (User_Id, ChatGroup_Id, Role, Is_Not_Notification, Delay_Until)
                              VALUES      
                              (@User_Id, @Group_Id, @Role, 0, GETDATE());";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    foreach (var item in command.Users)
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("User_Id", item.User_Id);
                        parameter.Add("Group_Id", id);
                        parameter.Add("Role", item.Role);
                        await connection.ExecuteAsync(query, parameter);
                    }
                    return ResponseModel<string>.Success("Success.");

                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> EditUserInChatGroup(Guid id, ModifyChatGroupUserCommand command)
        {
            var query = @"UPDATE User_ChatGroup
                          SET Role = @Role
                          WHERE ChatGroup_Id = @ChatGroup_Id AND User_Id = @User_Id";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameter = new DynamicParameters();
                    parameter.Add("User_Id", command.User_Id);
                    parameter.Add("ChatGroup_Id", id);
                    parameter.Add("Role", command.Role);
                    await connection.ExecuteAsync(query, parameter);
                    return ResponseModel<string>.Success("Success.");

                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> DeleteUserFromChatGroup(Guid id, DeleteChatGroupUsersCommand command)
        {
            var query = @"DELETE FROM User_ChatGroup
                          WHERE ChatGroup_Id = @ChatGroup_Id AND User_Id = @User_Id";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    foreach (var item in command.Users)
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("User_Id", item);
                        parameter.Add("ChatGroup_Id", id);
                        await connection.ExecuteAsync(query, parameter);
                    }
                    return ResponseModel<string>.Success("Success.");

                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> ModifyUserChatNotification(Guid id, ChangeNotificationCommand command)
        {
            var query = "UPDATE User_ChatGroup SET Is_Not_Notification = @Is_Not_Notification, Delay_Until = @Delay_Until WHERE User_Id = @User_Id AND ChatGroup_Id = @ChatGroup_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Is_Not_Notification", command.Is_Not_Notification);
            parameters.Add("Delay_Until", command.Delay_Until);
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("ChatGroup_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        #endregion

    }
}

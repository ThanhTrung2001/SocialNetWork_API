using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;
using System.Data;

namespace EV.DataAccess.Repositories
{
    public class GroupRepository
    {
        private readonly DatabaseContext _context;

        public GroupRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<GroupQuery>>> Get()
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
                    return ResponseModel<IEnumerable<GroupQuery>>.Success(groupDict.Values.ToList());
                }

            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<GroupQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<GroupQuery>> GetById(Guid id)
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
                    return ResponseModel<GroupQuery>.Success(groupDict.Values.ToList()[0] ?? null);
                }

            }
            catch (Exception ex)
            {
                return ResponseModel<GroupQuery>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<IEnumerable<GroupQuery>>> Search(string name)
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
                    return ResponseModel<IEnumerable<GroupQuery>>.Success(groupDict.Values.ToList());
                }

            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<GroupQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<IEnumerable<UserGroupQuery>>> GetUsersById(Guid id)
        {
            var query = @"SELECT ug.User_Id, ud.FirstName, ud.LastName, ud.Avatar as User_Avatar, ug.Role, ug.Joined_At 
                          FROM User_Details ud
                          LEFT JOIN User_Group ug ON ud.User_Id = ug.User_Id
                          WHERE ug.Group_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<UserGroupQuery>(query, parameter);

                    return ResponseModel<IEnumerable<UserGroupQuery>>.Success(result);
                }
                catch (Exception ex)
                {

                    return ResponseModel<IEnumerable<UserGroupQuery>>.Failure(ex.Message);
                }
            }
        }

        public async Task<ResponseModel<IEnumerable<PostQuery>>> GetPostsById(Guid id)
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
                            var existSurvey = postEntry.Survey ??= survey; // Initialize survey if null

                            if (surveyItem != null)
                            {
                                // Add SurveyItem if not exists
                                var existingItem = existSurvey.SurveyItems.FirstOrDefault(item => item.SurveyItem_Id == surveyItem.SurveyItem_Id);
                                if (existingItem == null)
                                {
                                    existingItem = surveyItem;
                                    existSurvey.SurveyItems.Add(existingItem);
                                }

                                // Add Vote if not exists in the correct SurveyItem
                                if (vote != null && !existingItem.Votes.Any(v => v.Vote_UserId == vote.Vote_UserId))
                                {
                                    existingItem.Votes.Add(vote);
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
                    return ResponseModel<IEnumerable<PostQuery>>.Success(postDict.Values.ToList());
                }

            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<PostQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<Guid>> Create(CreateGroupCommand command)
        {
            var query = @"INSERT INTO Groups (Id, Created_At, Updated_At, Is_Deleted, Name, Avatar, Wallpaper)
                        OUTPUT Inserted.Id
                        VALUES
                        (NEWID(), GETDATE(), GETDATE(), 0, @Name, @Avatar ,@Wallpaper);";
            var queryUser = @"INSERT INTO User_Group (User_Id, Group_Id, Role, Is_Follow ,Joined_At, Updated_At, Is_Deleted)
                              VALUES      
                              (@User_Id, @Id, @Role, 1, GETDATE(), GETDATE(),0);";
            var parameters = new DynamicParameters();
            parameters.Add("Name", command.Name, DbType.String);
            parameters.Add("Avatar", command.Avatar, DbType.String);
            parameters.Add("Wallpaper", command.Wallapper, DbType.String);
            parameters.Add("User_Id", command.Users[0].User_Id);
            parameters.Add("Role", command.Users[0].Role);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QuerySingleAsync<Guid>(query, parameters);
                    parameters.Add("Id", result, DbType.Guid);
                    await connection.ExecuteAsync(queryUser, parameters);
                    return ResponseModel<Guid>.Success(result);
                }
                catch (Exception ex)
                {
                    return ResponseModel<Guid>.Failure(ex.Message);
                }
            }
        }

        public async Task<ResponseModel<string>> Edit(Guid id, EditGroupCommand command)
        {
            var query = @"UPDATE Groups 
                        SET
                        Name = @Name, Avatar = @Avatar, Wallpaper = @Wallpaper
                        WHERE Id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Name", command.Name, DbType.String);
            parameters.Add("Avatar", command.Avatar, DbType.String);
            parameters.Add("Wallpaper", command.Wallapper, DbType.String);
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
            var query = "UPDATE Groups SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id;";
            var queryUserGroup = "UPDATE User_Group SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Group_Id = @Id;";
            var queryUserRequestGroup = "UPDATE User_Request_Group SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Group_Id = @Id;";
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
                        await connection.ExecuteAsync(queryUserRequestGroup, parameter, transaction);
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

        public async Task<ResponseModel<IEnumerable<GroupQuery>>> GetGroupsByUserId(Guid id)
        {
            var query = @"SELECT g.Id, g.Name, g.Avatar, g.Wallpaper, ud.User_Id, ud.FirstName, ud.LastName, ud.Avatar as User_Avatar, 
                          ug.Role, ug.Joined_At
                          FROM Groups g
                          LEFT JOIN User_Group ug ON g.Id = ug.Group_Id
                          LEFT JOIN User_Details ud ON ug.User_Id = ud.User_Id
                          WHERE ug.User_Id = @User_Id AND g.Is_Deleted = 0;";
            var parameter = new DynamicParameters();
            parameter.Add("User_Id", id);
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
                    return ResponseModel<IEnumerable<GroupQuery>>.Success(groupDict.Values.ToList());
                }

            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<GroupQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<string>> AddUser(Guid id, ModifyGroupUsersCommand command)
        {
            //check if exist
            var existQuery = @"SELECT COUNT(1) FROM User_Group
                               WHERE User_Id = @User_Id AND Group_Id = @Group_Id;";

            var query = @"INSERT INTO User_Group (User_Id, Group_Id, Role, Is_Follow, Joined_At, Updated_At ,Is_Deleted)
                              VALUES      
                              (@User_Id, @group_Id, @Role, @Is_Follow ,GETDATE(), GETDATE() ,0);";
            using (var connection = _context.CreateConnection())
            {
                foreach (var item in command.Users)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("User_Id", item.User_Id);
                    parameters.Add("Group_Id", id);
                    parameters.Add("Role", item.Role);
                    parameters.Add("Is_Follow", item.Is_Follow ?? true);
                    bool existed = await connection.ExecuteScalarAsync<bool>(existQuery, parameters);
                    if (existed)
                    {
                        return ResponseModel<string>.Failure("Existed Connection between User and Page")!;
                    }
                    else
                    {
                        await connection.ExecuteAsync(query, parameters);
                    }
                }
                return ResponseModel<string>.Success("Success");
            }
        }

        public async Task<ResponseModel<string>> EditUser(Guid id, ModifyGroupUserCommand command)
        {
            var queryUserGroup = "UPDATE User_Group SET Is_Follow = @Is_Follow, Role = @Role WHERE Group_Id = @Id AND User_Id = @User_Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Role", command.Role);
            parameters.Add("Is_Follow", command.Is_Follow);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(queryUserGroup, parameters, transaction);
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

        public async Task<ResponseModel<string>> DeleteUser(Guid id, DeleteGroupUsersCommand command)
        {
            var query = "UPDATE User_Group SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Group_Id = @Group_Id AND User_Id = @User_Id;";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    foreach (var item in command.Users)
                    {
                        var parameter = new DynamicParameters();
                        parameter.Add("Group_Id", id, DbType.Guid);
                        parameter.Add("User_Id", item.User_Id);
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

        #endregion
    }
}

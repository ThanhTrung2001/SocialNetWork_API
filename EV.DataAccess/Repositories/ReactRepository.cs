using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;
using System.Data;

namespace EV.DataAccess.Repositories
{
    public class ReactRepository
    {
        private readonly DatabaseContext _context;

        public ReactRepository(DatabaseContext context)
        {
            _context = context;
        }

        #region [Post]
        public async Task<ResponseModel<IEnumerable<PostReactQuery>>> GetByPostId(Guid id)
        {
            var query = @"SELECT 
                 urp.React_Type,
                 urp.User_Id AS React_UserId,
                 urp.Is_SharePost,
                 ud.FirstName AS React_FirstName,
                 ud.LastName AS React_LastName,
                 ud.Avatar AS React_Avatar
                 FROM User_React_Post urp
                 LEFT JOIN User_Details ud ON urp.User_Id = ud.User_Id
                 WHERE 
                 urp.Is_Deleted = 0 AND urp.Post_Id = @Post_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Post_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<PostReactQuery>(query, parameters);
                    return ResponseModel<IEnumerable<PostReactQuery>>.Success(result);
                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<PostReactQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> CreatePostReact(CreatePostReactCommand command)
        {
            var query = @"INSERT INTO User_React_Post (User_Id, Post_Id, React_Type, Is_SharePost,Created_At, Updated_At, Is_Deleted)
                        VALUES 
                        (@User_Id, @Post_Id, @React_Type, @Is_SharePost ,GETDATE(), GETDATE(), 0)";
            var updatePostQuery = "UPDATE Posts SET React_Count = React_Count + 1 WHERE Id = @Post_Id";
            var updateSharePostQuery = "UPDATE Share_Posts SET React_Count = React_Count + 1 WHERE Id = @Post_Id";
            var parameter = new DynamicParameters();
            parameter.Add("User_Id", command.User_Id, DbType.Guid);
            parameter.Add("Post_Id", command.Post_Id);
            parameter.Add("React_Type", command.React_Type);
            parameter.Add("Is_SharePost", command.Is_SharePost);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameter, transaction);
                        if (command.Is_SharePost == false)
                        {
                            await connection.ExecuteAsync(updatePostQuery, parameter, transaction);
                        }
                        else
                        {
                            await connection.ExecuteAsync(updateSharePostQuery, parameter, transaction);
                        }
                        transaction.Commit();
                        return ResponseModel<string>.Success("Success");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ResponseModel<string>.Failure(ex.Message)!;
                    }
                }
            }
        }

        public async Task<ResponseModel<string>> EditPostReact(Guid id, EditReactCommand command)
        {
            var query = @"UPDATE User_React_Post SET React_Type = @React_Type WHERE Post_Id = @Id AND User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("React_Type", command.React_Type);
            parameters.Add("User_Id", command.User_Id);
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

        public async Task<ResponseModel<string>> DeletePostReact(Guid id, DeletePostReactCommand command)
        {
            var query = "Update User_React_Post SET Is_Deleted = 1, Updated_At = GETDATE() Where User_Id = @User_Id AND Post_Id = @Post_Id";
            var updatePostQuery = "UPDATE Posts SET React_Count = React_Count - 1 WHERE Id = @Post_Id";
            var updateSharePostQuery = "UPDATE Share_Posts SET React_Count = React_Count - 1 WHERE Id = @Post_Id";
            var parameter = new DynamicParameters();
            parameter.Add("User_Id", command.User_Id);
            parameter.Add("Post_Id", id);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = await connection.ExecuteAsync(query, parameter, transaction);
                        if (command.Is_SharePost == false)
                        {
                            await connection.ExecuteAsync(updatePostQuery, parameter, transaction);
                        }
                        else
                        {
                            await connection.ExecuteAsync(updateSharePostQuery, parameter, transaction);
                        }
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
        #endregion

        #region [Comment]
        public async Task<ResponseModel<IEnumerable<CommentReactQuery>>> GetByCommentId(Guid id)
        {
            var query = @"SELECT 
                 urc.React_Type,
                 urc.User_Id AS React_UserId,
                 ud.FirstName AS React_FirstName,
                 ud.LastName AS React_LastName,
                 ud.Avatar AS React_Avatar
                 FROM User_React_Comment urc
                 LEFT JOIN User_Details ud ON urc.User_Id = ud.User_Id
                 WHERE 
                 urc.Is_Deleted = 0 AND urc.Comment_Id = @Comment_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Comment_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<CommentReactQuery>(query, parameters);
                    return ResponseModel<IEnumerable<CommentReactQuery>>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<CommentReactQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> CreateCommentReact(CreateCommentReactCommand command)
        {
            var query = @"INSERT INTO User_React_Comment (User_Id, Comment_Id, React_Type, Created_At, Updated_At, Is_Deleted)
                        VALUES 
                        (@User_Id, @Comment_Id, @React_Type, GETDATE(), GETDATE(), 0)";
            var updateQuery = "UPDATE Comments SET React_Count = React_Count + 1 WHERE Id = @Comment_Id";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id, DbType.Guid);
            parameters.Add("Comment_Id", command.Comment_Id);
            parameters.Add("React_Type", command.React_Type);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameters, transaction);
                        await connection.ExecuteAsync(updateQuery, parameters, transaction);
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

        public async Task<ResponseModel<string>> EditCommentReact(Guid id, EditReactCommand command)
        {
            var query = @"UPDATE User_React_Comment SET React_Type = @React_Type WHERE Comment_Id = @Id AND User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("React_Type", command.React_Type);
            parameters.Add("User_Id", command.User_Id);
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

        public async Task<ResponseModel<string>> DeleteCommentReact(Guid id, DeleteReactCommand command)
        {
            var query = "Update User_React_Comment SET Is_Deleted = 1, Updated_At = GETDATE() Where User_Id = @User_Id AND Post_Id = @Post_Id";
            var updateQuery = "UPDATE Comments SET React_Count = React_Count - 1 WHERE Id = @Comment_Id";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Comment_Id", id);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameters, transaction);
                        await connection.ExecuteAsync(updateQuery, parameters, transaction);
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
        #endregion

        #region [Message]
        public async Task<ResponseModel<IEnumerable<MessageReactQuery>>> GetByMessageId(Guid id)
        {
            var query = @"SELECT 

                 urm.React_Type,
                 urm.User_Id AS React_UserId,
                 ud.FirstName AS React_FirstName,
                 ud.LastName AS React_LastName,
                 ud.Avatar AS React_Avatar
                 FROM User_React_Message urm
                 LEFT JOIN User_Details ud ON urm.User_Id = ud.User_Id
                 WHERE 
                 urm.Is_Deleted = 0 AND urm.Message_Id = @Message_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Message_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<MessageReactQuery>(query, parameters);
                    return ResponseModel<IEnumerable<MessageReactQuery>>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<MessageReactQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> CreateMessageReact(CreateMessageReactCommand command)
        {
            var query = @"INSERT INTO User_React_Message (User_Id, Message_Id, React_Type, Created_At, Updated_At, Is_Deleted)
                        VALUES 
                        (@User_Id, @Message_Id, @React_Type,GETDATE(), GETDATE(), 0)";
            var updateQuery = "UPDATE Messages SET React_Count = React_Count + 1 WHERE Id = @Message_Id";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id, DbType.Guid);
            parameters.Add("Message_Id", command.Message_Id);
            parameters.Add("React_Type", command.React_Type);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameters, transaction);
                        await connection.ExecuteAsync(updateQuery, parameters, transaction);
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

        public async Task<ResponseModel<string>> EditMessageReact(Guid id, EditReactCommand command)
        {
            var query = @"UPDATE User_React_Message SET React_Type = @React_Type WHERE Message_Id = @Id AND User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("React_Type", command.React_Type);
            parameters.Add("User_Id", command.User_Id);
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

        public async Task<ResponseModel<string>> DeleteMessageReact(Guid id, DeleteReactCommand command)
        {
            var query = "Update User_React_Message SET Is_Deleted = 1, Updated_At = GETDATE() Where User_Id = @User_Id AND Post_Id = @Post_Id";
            var updateQuery = "UPDATE Messages SET React_Count = React_Count - 1 WHERE Id = @Message_Id";

            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Message_Id", id);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameters, transaction);
                        await connection.ExecuteAsync(updateQuery, parameters, transaction);
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
        #endregion

    }
}

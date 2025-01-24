using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;
using System.Data;

namespace EV.DataAccess.Repositories
{
    public class CommentRepository
    {
        private readonly DatabaseContext _context;

        public CommentRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<CommentQuery>>> GetByPostId(Guid id)
        {
            var query = @"SELECT 
                            c.Id, c.Content, c.Is_Response, c.Is_SharePost,c.React_Count, c.Updated_At, c.User_Id,
                            ud.FirstName, ud.LastName, ud.Avatar,
                            a.Id AS Attachment_Id, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN User_Details ud ON c.User_Id = ud.User_Id 
                          LEFT JOIN
                            Comment_Attachment ca ON ca.Comment_Id = c.Id
                          LEFT JOIN
                            Attachments a ON ca.Attachment_Id = a.Id
                          Where c.Post_Id = @Id AND c.Is_Deleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var commentDict = new Dictionary<Guid, CommentQuery>();
                using (var connection = _context.CreateConnection())
                {

                    var result = await connection.QueryAsync<CommentQuery, AttachmentQuery, CommentQuery>(
                        query,
                    map: (comment, attachment) =>
                    {
                        if (!commentDict.TryGetValue(comment.Id, out var commentEntry))
                        {
                            commentEntry = comment;
                            commentDict.Add(comment.Id, commentEntry);
                        }

                        if (attachment != null && !commentEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            commentEntry.Attachments.Add(attachment);
                        }
                        return commentEntry;
                    },
                        parameter,
                        splitOn: "Attachment_Id");
                    return ResponseModel<IEnumerable<CommentQuery>>.Success(commentDict.Values.ToList());
                }

            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<CommentQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<IEnumerable<CommentDetailQuery>>> GetDetailByPostId(Guid id)
        {
            var query = @"SELECT 
                          c.Id, c.Content, c.Is_Response, c.Is_SharePost, c.React_Count, c.Updated_At, c.User_Id, 
                          ud.FirstName, ud.LastName, ud.Avatar,
                          urc.React_Type, urc.User_Id as React_UserId, urc.Created_At, 
                          ur.FirstName AS React_FirstName, ur.LastName AS React_LastName, ur.Avatar AS React_Avatar,
                          a.Id AS Attachment_Id, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN User_Details ud ON c.User_Id = ud.User_Id 
                          LEFT JOIN User_React_Comment urc ON c.Id = urc.Comment_Id
                          LEFT JOIN User_Details ur ON urc.User_Id = ur.User_Id
                          LEFT JOIN
                            Comment_Attachment ca ON ca.Comment_Id = c.Id
                          LEFT JOIN
                            Attachments a ON ca.Attachment_Id = a.Id
                          Where c.Post_Id = @Id AND c.Is_Deleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var commentDict = new Dictionary<Guid, CommentDetailQuery>();
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<CommentDetailQuery, CommentReactQuery, AttachmentQuery, CommentDetailQuery>(query,
                        map: (comment, react, attachment) =>
                        {
                            if (!commentDict.TryGetValue(comment.Id, out var commentEntry))
                            {
                                commentEntry = comment;
                                commentDict.Add(comment.Id, commentEntry);
                            }
                            if (react != null && !commentEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                            {
                                commentEntry.Reacts.Add(react);
                            }
                            if (attachment != null && !commentEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                            {
                                commentEntry.Attachments.Add(attachment);
                            }
                            return commentEntry;
                        },
                        parameter,
                        splitOn: "React_Type, Attachment_Id");
                    return ResponseModel<IEnumerable<CommentDetailQuery>>.Success(commentDict.Values.ToList());
                }

            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<CommentDetailQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<CommentDetailQuery>> GetById(Guid id)
        {
            var query = @"SELECT 
                          c.Id, c.Content, c.Is_Response, c.React_Count, c.Updated_At, c.User_Id, 
                          ud.FirstName, ud.LastName, ud.Avatar,
                          urc.React_Type, urc.User_Id as React_UserId, urc.Created_At,
                          ur.FirstName AS React_FirstName, ur.LastName AS React_LastName, ur.Avatar AS React_Avatar,
                          a.Id AS Attachment_Id, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN User_Details ud ON c.User_Id = ud.User_Id 
                          LEFT JOIN User_React_Comment urc ON c.Id = urc.Comment_Id
                          LEFT JOIN User_Details ur ON urc.User_Id = ur.User_Id
                          LEFT JOIN
                            Comment_Attachment ca ON ca.Comment_Id = c.Id
                          LEFT JOIN
                            Attachments a ON ca.Attachment_Id = a.Id
                          Where c.Id = @Id AND c.Is_Deleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var commentDict = new Dictionary<Guid, CommentDetailQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<CommentDetailQuery, CommentReactQuery, AttachmentQuery, CommentDetailQuery>(
                    query,
                    map: (comment, react, attachment) =>
                    {
                        if (!commentDict.TryGetValue(comment.Id, out var commentEntry))
                        {
                            commentEntry = comment;
                            commentDict.Add(comment.Id, commentEntry);
                        }

                        if (react != null && !commentEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                        {
                            commentEntry.Reacts.Add(react);
                        }
                        if (attachment != null && !commentEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            commentEntry.Attachments.Add(attachment);
                        }
                        return commentEntry;
                    },
                    parameter,
                    splitOn: "React_Type, Attachment_Id");
                }
                return ResponseModel<CommentDetailQuery>.Success(commentDict.Values.ToList()[0] ?? null);

            }
            catch (Exception ex)
            {
                return ResponseModel<CommentDetailQuery>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<IEnumerable<CommentDetailQuery>>> GetResponseById(Guid id)
        {
            var query = @"SELECT 
                          c.Id, c.Content, c.Is_Response, c.React_Count, c.Updated_At, c.User_Id, 
                          ud.FirstName, ud.LastName , ud.Avatar,
                          urc.React_Type, urc.User_Id as React_UserId, urc.Created_At, 
                          ur.FirstName AS React_FirstName, ur.LastName AS React_LastName, ur.Avatar AS React_Avatar,
                          a.Id AS Attachment_Id, a.Media, a.Description
                          FROM Comments c
                          INNER JOIN User_Details ud ON c.User_Id = ud.User_Id 
                          LEFT JOIN User_React_Comment urc ON c.Id = urc.Comment_Id
                          LEFT JOIN User_Details ur ON urc.User_Id = ur.User_Id
                          LEFT JOIN Comment_Response cmr ON c.Id = cmr.Response_Id
                          LEFT JOIN
                            Comment_Attachment ca ON ca.Comment_Id = c.Id
                          LEFT JOIN
                            Attachments a ON ca.Attachment_Id = a.Id
                          Where c.Is_Deleted = 0 AND c.Is_Response = 1 AND cmr.Comment_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var commentDict = new Dictionary<Guid, CommentDetailQuery>();
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<CommentDetailQuery, CommentReactQuery, AttachmentQuery, CommentDetailQuery>(query,
                        map: (comment, react, attachment) =>
                        {
                            if (!commentDict.TryGetValue(comment.Id, out var commentEntry))
                            {
                                commentEntry = comment;
                                commentDict.Add(comment.Id, commentEntry);
                            }
                            if (react != null && !commentEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                            {
                                commentEntry.Reacts.Add(react);
                            }
                            if (attachment != null && !commentEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                            {
                                commentEntry.Attachments.Add(attachment);
                            }
                            return commentEntry;
                        },
                        parameter,
                        splitOn: "React_Type, Attachment_Id");
                    return ResponseModel<IEnumerable<CommentDetailQuery>>.Success(commentDict.Values.ToList());
                }

            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<CommentDetailQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<Guid>> Create(CreateCommentCommand command)
        {
            var query = @"INSERT INTO Comments (Id, Created_At, Updated_At, Is_Deleted, Content, Is_Response, React_Count ,User_Id, Post_Id)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Content, @Is_Response, 0, @User_Id, @Post_Id)";
            var queryAttachment = @"INSERT INTO Attachments (Id, Media, Description)
                                    OUTPUT Inserted.Id
                                    VALUES
                                    (NEWID(), @Media, @Description)";
            var queryComment_Attachment = @"INSERT INTO Comment_Attachment (Comment_Id, Attachment_Id)
                                        VALUES
                                        (@Comment_Id, @Attachment_Id)";
            var queryResponse = @"INSERT INTO Comment_Response (Comment_Id, Response_Id)
                                  VALUES
                                  (@Comment_Id, @Response_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("Content", command.Content, DbType.String);
            parameters.Add("User_Id", command.User_Id, DbType.Guid);
            parameters.Add("Post_Id", command.Post_Id);
            parameters.Add("Is_Response", command.Is_Response);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = await connection.QuerySingleAsync<Guid>(query, parameters, transaction);
                        if (command.Attachments != null)
                        {
                            foreach (var item in command.Attachments!)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("Media", item.Media);
                                parameters.Add("Description", item.Description);
                                var attachmentResult = await connection.QuerySingleAsync<Guid>(queryAttachment, parameters, transaction);
                                parameters.Add("Comment_Id", result);
                                parameters.Add("Attachment_Id", attachmentResult);
                                await connection.ExecuteAsync(queryComment_Attachment, parameters, transaction);
                            }
                        }
                        if (command.Is_Response == true)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("Comment_Id", command.Comment_Id);
                            parameters.Add("Response_Id", result);
                            await connection.ExecuteAsync(queryResponse, parameters, transaction);
                        }

                        transaction.Commit();
                        return ResponseModel<Guid>.Success(result);
                    }

                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ResponseModel<Guid>.Failure(ex.Message)!;
                    }
                }
            }
        }

        public async Task<ResponseModel<string>> Edit(Guid id, EditCommentCommand command)
        {
            var query = "UPDATE Comments SET Content = @Content, Updated_At = GETDATE() WHERE Id = @Id AND User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Content", command.Content, DbType.String);
            parameters.Add("User_Id", command.User_Id, DbType.Guid);
            parameters.Add("Id", id, DbType.Guid);
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
            var query = "Update Comments SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id";
            var queryReact = "Update User_React_Comment SET  Is_Deleted = 1, Updated_At = GETDATE() WHERE Comment_Id = @Id";
            var queryAttachment = "Update Attachments SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id IN (SELECT Attachment_Id FROM Comment_Attachment WHERE Comment_Id = @Id)";
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
                        await connection.ExecuteAsync(queryReact, parameter, transaction);
                        await connection.ExecuteAsync(queryAttachment, parameter, transaction);
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
    }
}

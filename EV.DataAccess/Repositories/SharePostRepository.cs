using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;
using System.Data;

namespace EV.DataAccess.Repositories
{
    public class SharePostRepository
    {
        private readonly DatabaseContext _context;

        public SharePostRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<SharePostQuery>>> Get()
        {
            try
            {
                var shareDict = new Dictionary<Guid, SharePostQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<SharePostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, SharePostQuery>(
                    "GetSharePosts",
                    map: (share, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!shareDict.TryGetValue(share.Id, out var shareEntry))
                        {
                            shareEntry = share;
                            shareDict.Add(share.Id, shareEntry);
                        }

                        if (share.Post_Type == "Normal" && attachment != null && !shareEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            shareEntry.Attachments.Add(attachment);
                        }

                        if (share.Post_Type == "Survey" && survey != null)
                        {
                            var existSurvey = shareEntry.Survey ??= survey; // Initialize survey if null

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
                        if (comment != null && !shareEntry.Comments.Any((item) => item.Comment_Id == comment.Comment_Id))
                        {
                            shareEntry.Comments.Add(comment);
                        }
                        if (react != null && !shareEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                        {
                            shareEntry.Reacts.Add(react);
                        }
                        return shareEntry;
                    },
                    commandType: CommandType.StoredProcedure,
                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
                    return ResponseModel<IEnumerable<SharePostQuery>>.Success(shareDict.Values.ToList());
                }
            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<SharePostQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<IEnumerable<SharePostQuery>>> GetByUserId(Guid id)
        {
            try
            {
                var shareDict = new Dictionary<Guid, SharePostQuery>();
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<SharePostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, SharePostQuery>(
                    "GetSharePostsByUserId",
                    map: (share, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!shareDict.TryGetValue(share.Id, out var shareEntry))
                        {
                            shareEntry = share;
                            shareDict.Add(share.Id, shareEntry);
                        }

                        if (share.Post_Type == "Normal" && attachment != null && !shareEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            shareEntry.Attachments.Add(attachment);
                        }

                        if (share.Post_Type == "Survey" && survey != null)
                        {
                            var existSurvey = shareEntry.Survey ??= survey; // Initialize survey if null

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

                        if (comment != null && !shareEntry.Comments.Any((item) => item.Comment_Id == comment.Comment_Id))
                        {
                            shareEntry.Comments.Add(comment);
                        }
                        if (react != null && !shareEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                        {
                            shareEntry.Reacts.Add(react);
                        }
                        return shareEntry;
                    },
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
                    return ResponseModel<IEnumerable<SharePostQuery>>.Success(shareDict.Values.ToList());
                }
            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<SharePostQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<SharePostQuery>> GetById(Guid id)
        {
            try
            {
                var shareDict = new Dictionary<Guid, SharePostQuery>();
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<SharePostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, SharePostQuery>(
                    "GetSharePostsById",
                    map: (share, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!shareDict.TryGetValue(share.Id, out var shareEntry))
                        {
                            shareEntry = share;
                            shareDict.Add(share.Id, shareEntry);
                        }

                        if (share.Post_Type == "Normal" && attachment != null && !shareEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            shareEntry.Attachments.Add(attachment);
                        }

                        if (share.Post_Type == "Survey" && survey != null)
                        {
                            var existSurvey = shareEntry.Survey ??= survey; // Initialize survey if null

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

                        if (comment != null && !shareEntry.Comments.Any((item) => item.Comment_Id == comment.Comment_Id))
                        {
                            shareEntry.Comments.Add(comment);
                        }
                        if (react != null && !shareEntry.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                        {
                            shareEntry.Reacts.Add(react);
                        }
                        return shareEntry;
                    },
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
                    return ResponseModel<SharePostQuery>.Success(shareDict.Values.ToList()[0] ?? null);
                }
            }
            catch (Exception ex)
            {
                return ResponseModel<SharePostQuery>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<IEnumerable<ShareUserQuery>>> GetShareUsersByPostId(Guid id)
        {
            var query = @" SELECT s.Id, s.Shared_By_User_Id AS Share_UserId, ud.FirstName AS Share_FirstName, ud.LastName AS Share_LastName, ud.Avatar AS Share_Avatar
                           FROM Share_Posts s
                           LEFT JOIN User_Details ud ON s.Shared_By_User_Id = ud.User_Id
                           WHERE s.Shared_Post_Id = @Post_Id AND s.Is_Deleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Post_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<ShareUserQuery>(query, parameter);
                    return ResponseModel<IEnumerable<ShareUserQuery>>.Success(result);
                }

                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<ShareUserQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<Guid>> Create(CreateSharePostCommand command)
        {
            var query = @"INSERT INTO Share_Posts (Id, Created_At, Updated_At, Is_Deleted, Shared_Post_Id ,Shared_By_User_Id, Content, In_Group, Destination_Id)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Post_Id, @Shared_By_User_Id, @Content, @In_Group, @Destination_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("Post_Id", command.Post_Id);
            parameters.Add("Shared_By_User_Id", command.Shared_By_User_Id);
            parameters.Add("Content", command.Content);
            parameters.Add("In_Group", command.In_Group);
            parameters.Add("Destination_Id", command.Destination_Id);

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

        public async Task<ResponseModel<string>> Edit(Guid id, EditSharePostCommand command)
        {
            var query = @"UPDATE Share_Posts SET Content = @Content WHERE Id = @Id AND Shared_By_User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("User_Id", command.Shared_By_User_Id);
            parameters.Add("Content", command.Share_Content);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return ResponseModel<string>.Success("Success.");

                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Delete(Guid id)
        {
            var query = "UPDATE Share_Posts SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id;";
            var queryReact = "Update User_React_Post SET  Is_Deleted = 1, Updated_At = GETDATE() WHERE Post_Id = @Id AND Is_SharePost = 1";
            var queryAttachment = "Update Attachments SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id IN (SELECT Attachment_Id FROM Post_Attachment WHERE Post_Id = @Id)";
            var queryComment = "Update Comments SET  Is_Deleted = 1, Updated_At = GETDATE() WHERE Post_Id = @Id AND Is_SharePost = 1";
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
                        await connection.ExecuteAsync(queryComment, parameter, transaction);
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

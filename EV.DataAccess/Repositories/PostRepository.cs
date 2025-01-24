using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;
using System.Data;

namespace EV.DataAccess.Repositories
{
    public class PostRepository
    {
        private readonly DatabaseContext _context;

        public PostRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<PostQuery>>> Get()
        {
            try
            {
                var postDict = new Dictionary<Guid, PostQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, PostQuery>(
                    "GetPosts",
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

        public async Task<ResponseModel<IEnumerable<PostQuery>>> GetByUserId(Guid id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
                var postDict = new Dictionary<Guid, PostQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, PostQuery>(
                    "GetPostsByUserId",
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

        public async Task<ResponseModel<PostQuery>> GetById(Guid id)
        {
            try
            {
                var postDict = new Dictionary<Guid, PostQuery>();
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);


                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, PostQuery>(
                    "GetPostById",
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
                    return ResponseModel<PostQuery>.Success(postDict.Values.ToList()[0] ?? null);
                }

            }
            catch (Exception ex)
            {
                return ResponseModel<PostQuery>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<Guid>> Create(CreatePostRequest command)
        {
            var queryPost = @"INSERT INTO Posts (Id, Created_At, Updated_At, Is_Deleted, Destination_Id, In_Group, Post_Type, Content, User_Id, React_Count)
                              OUTPUT Inserted.Id
                              VALUES 
                              (NEWID(), GETDATE(), GETDATE(), 0, @Destination_Id, @In_Group, @Post_Type, @Content, @User_Id, 0);";
            var queryAttachment = @"INSERT INTO Attachments (Id, Created_At, Updated_At, Is_Deleted, Media, Description)
                               OUTPUT Inserted.Id
                               VALUES 
                               (NEWID(), GETDATE(), GETDATE(), 0, @Media, @Description)";
            var queryPost_Attachment = @"INSERT INTO Post_Attachment (Post_Id, Attachment_Id)
                               VALUES 
                               (@Id, @Attachment_Id)";
            var querySurvey = @"INSERT INTO Surveys (ID, Created_At, Updated_At, Is_Deleted, Expired_At, Total_Vote, Post_Id ,Survey_Type, Question)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Expired_At, 0, @Id ,@Survey_Type ,@Question)";
            var querySurveyItem = @"INSERT INTO Survey_Items (ID, Created_At, Updated_At, Is_Deleted, Option_Name, Total_Vote, Survey_Id)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Option_Name, 0, @Survey_Id)";

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("In_Group", command.NewPost.In_Group);
                        parameters.Add("Destination_Id", command.NewPost.Destination_Id);
                        parameters.Add("Post_Type", command.NewPost.Post_Type);
                        parameters.Add("Content", command.NewPost.Content);
                        parameters.Add("User_Id", command.User_Id);
                        //insert post first
                        var resultPost = await connection.QuerySingleAsync<Guid>(queryPost, parameters, transaction);
                        if (command.NewPost.Post_Type == "Normal")
                        {
                            foreach (var item in command.NewPost.Attachments)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("Id", resultPost);
                                parameters.Add("Media", item.Media);
                                parameters.Add("Description", item.Description);
                                var resultAttachment = await connection.QuerySingleAsync<Guid>(queryAttachment, parameters, transaction);
                                parameters.Add("Attachment_Id", resultAttachment);
                                await connection.ExecuteAsync(queryPost_Attachment, parameters, transaction);
                            }
                        }
                        else if (command.NewPost.Post_Type == "Survey" && command.NewPost.Survey.SurveyItems != null)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("Id", resultPost);
                            parameters.Add("Expired_At", command.NewPost.Survey.Expired_At);
                            parameters.Add("Question", command.NewPost.Survey.Question);
                            parameters.Add("Survey_Type", command.NewPost.Survey.Survey_Type);
                            var resultSurvey = await connection.QuerySingleAsync<Guid>(querySurvey, parameters, transaction);
                            foreach (var item in command.NewPost.Survey.SurveyItems)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("Survey_Id", resultSurvey);
                                parameters.Add("Option_Name", item.Option_Name);
                                await connection.ExecuteAsync(querySurveyItem, parameters, transaction);
                            }
                        }
                        transaction.Commit();
                        return ResponseModel<Guid>.Success(resultPost);

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ResponseModel<Guid>.Failure(ex.Message)!;
                    }
                }
            }
        }

        public async Task<ResponseModel<string>> Edit(Guid id, EditPostRequest command)
        {
            var query = @"UPDATE Posts
                          SET Post_Type = @Post_Type, Content = @Content 
                          WHERE Id = @Id AND User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Post_Type", command.Post_Type);
            parameters.Add("Content", command.Content);
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Id", id);
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
            var query = "Update Posts SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id";
            var queryReact = "Update User_React_Post SET  Is_Deleted = 1, Updated_At = GETDATE() WHERE Post_Id = @Id";
            var queryAttachment = "Update Attachments SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id IN (SELECT Attachment_Id FROM Post_Attachment WHERE Post_Id = @Id)";
            var queryComment = "Update Comments SET  Is_Deleted = 1, Updated_At = GETDATE() WHERE Post_Id = @Id";
            var querySurvey = "Update Surveys SET Is_Deleted = 1 WHERE Post_Id = @Id";
            var querySurveyItem = "Update Survey_Items SET Updated_At = GETDATE(), Is_Deleted = 1 WHERE Survey_Id = (SELECT Id From Surveys WHERE Post_Id = @Id)";
            var querySharePost = "UPDATE Share_Posts SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Shared_Post_Id = @Id;";
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
                        await connection.ExecuteAsync(querySurvey, parameter, transaction);
                        await connection.ExecuteAsync(querySurveyItem, parameter, transaction);
                        await connection.ExecuteAsync(querySharePost, parameter, transaction);
                        //await connection.ExecuteAsync(querySharePost, parameters);
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

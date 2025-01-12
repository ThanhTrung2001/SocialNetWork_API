using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
using EnVietSocialNetWorkAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly DapperContext _context;

        public PostsController(DapperContext context)
        {
            _context = context;
        }

        // GET: api/<PostController>
        [HttpGet]
        public async Task<IActionResult> Get()
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

                        if (post.Post_Type_Id == 1 && attachment != null && !postEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            postEntry.Attachments.Add(attachment);
                        }

                        if (post.Post_Type_Id == 2 && survey != null)
                        {
                            postEntry.Survey = survey;
                            if (surveyItem != null && !postEntry.Survey.SurveyItems.Any((item) => item.SurveyItem_Id == surveyItem.SurveyItem_Id))
                            {
                                postEntry.Survey.SurveyItems.Add(surveyItem);
                                var result = postEntry.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItem_Id == surveyItem.SurveyItem_Id);
                                if (vote != null && !result.Votes.Any((item) => item.Vote_UserId == vote.Vote_UserId))
                                {
                                    result.Votes.Add(vote);
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
                    return Ok(ResponseModel<IEnumerable<PostQuery>>.Success(postDict.Values.ToList()));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<PostQuery>>.Failure(ex.Message));
            }
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserPostByUserID(Guid id)
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

                        if (post.Post_Type_Id == 1 && attachment != null && !postEntry.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            postEntry.Attachments.Add(attachment);
                        }

                        if (post.Post_Type_Id == 2 && survey != null)
                        {
                            postEntry.Survey = survey;
                            if (surveyItem != null && !postEntry.Survey.SurveyItems.Any((item) => item.SurveyItem_Id == surveyItem.SurveyItem_Id))
                            {
                                postEntry.Survey.SurveyItems.Add(surveyItem);
                                var result = postEntry.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItem_Id == surveyItem.SurveyItem_Id);
                                if (vote != null && !result.Votes.Any((item) => item.Vote_UserId == vote.Vote_UserId))
                                {
                                    result.Votes.Add(vote);
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
                    return Ok(ResponseModel<IEnumerable<PostQuery>>.Success(postDict.Values.ToList()));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<PostQuery>>.Failure(ex.Message));
            }
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            try
            {
                PostQuery postBasic = null;
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);


                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, PostQuery>(
                    "GetPostById",
                    map: (post, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (postBasic == null)
                        {
                            postBasic = post;
                        }

                        if (post.Post_Type_Id == 1 && attachment != null && !postBasic.Attachments.Any((item) => item.Attachment_Id == attachment.Attachment_Id))
                        {
                            postBasic.Attachments.Add(attachment);
                        }

                        if (post.Post_Type_Id == 2 && survey != null)
                        {
                            postBasic.Survey = survey;
                            if (surveyItem != null && !postBasic.Survey.SurveyItems.Any((item) => item.SurveyItem_Id == surveyItem.SurveyItem_Id))
                            {
                                postBasic.Survey.SurveyItems.Add(surveyItem);
                                var result = postBasic.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItem_Id == surveyItem.SurveyItem_Id);
                                if (vote != null && !result.Votes.Any((item) => item.Vote_UserId == vote.Vote_UserId))
                                {
                                    result.Votes.Add(vote);
                                }
                            }
                        }

                        if (comment != null && !postBasic.Comments.Any((item) => item.Comment_Id == comment.Comment_Id))
                        {
                            postBasic.Comments.Add(comment);
                        }
                        if (react != null && !postBasic.Reacts.Any((item) => item.React_UserId == react.React_UserId))
                        {
                            postBasic.Reacts.Add(react);
                        }
                        return postBasic;
                    },
                    parameter,
                    commandType: CommandType.StoredProcedure,
                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
                    return Ok(ResponseModel<PostQuery>.Success(postBasic));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<PostQuery>.Failure(ex.Message));
            }
        }

        // POST api/<PostController>
        [HttpPost]
        public async Task<IActionResult> Post(CreatePostRequest request)
        {
            var newPost = request.NewPost;
            var queryPost = @"INSERT INTO Posts (Id, Created_At, Updated_At, Is_Deleted, Destination_Id, In_Group, Post_Type_Id, Content, User_Id, React_Count)
                              OUTPUT Inserted.Id
                              VALUES 
                              (NEWID(), GETDATE(), GETDATE(), 0, @Destination_Id, @In_Group, @Post_Type_Id, @Content, @User_Id, 0);";
            var queryAttachment = @"INSERT INTO Attachments (Id, Created_At, Updated_At, Is_Deleted, Media, Description)
                               OUTPUT Inserted.Id
                               VALUES 
                               (NEWID(), GETDATE(), GETDATE(), 0, @Media, @Description)";
            var queryPost_Attachment = @"INSERT INTO Post_Attachment (Post_Id, Attachment_Id)
                               VALUES 
                               (@Id, @Attachment_Id)";
            var querySurvey = @"INSERT INTO Surveys (ID, Created_At, Updated_At, Is_Deleted, Expired_At, Total, Survey_Type, Id, Question)
                                OUTPUT Inserted.Id
                                VALUES
                                (NEWID(), GETDATE(), GETDATE(), 0, @Expired_At, 0, @Survey_Type, @Id, @Question)";
            var querySurveyItem = @"INSERT INTO Survey_Items (ID, Created_At, Updated_At, Is_Deleted, Option_Name, Total, Survey_Id)
                                    OUTPUT Inserted.Id
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
                        parameters.Add("In_Group", newPost.In_Group);
                        parameters.Add("Destination_Id", newPost.Destination_Id);
                        parameters.Add("Post_Type_Id", newPost.Post_Type_Id);
                        parameters.Add("Content", newPost.Content);
                        parameters.Add("UserId", request.UserId);
                        //insert post first
                        var resultPost = await connection.QuerySingleAsync<Guid>(queryPost, parameters, transaction);
                        if (request.NewPost.Post_Type_Id == 1)
                        {
                            foreach (var item in request.NewPost.Attachments)
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
                        else if (request.NewPost.Post_Type_Id == 2)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("Id", resultPost);
                            parameters.Add("Expired_At", request.NewPost.Survey.Expired_At);
                            parameters.Add("Question", request.NewPost.Survey.Question);
                            parameters.Add("Survey_Type", request.NewPost.Survey.Survey_Type);
                            var resultSurvey = await connection.QuerySingleAsync<Guid>(querySurvey, parameters, transaction);
                            foreach (var item in request.NewPost.Survey.SurveyItems)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("Survey_Id", resultSurvey);
                                parameters.Add("Option_Name", item.Option_Name);
                                await connection.ExecuteAsync(querySurveyItem, parameters, transaction);
                            }
                        }
                        transaction.Commit();
                        return Ok(ResponseModel<Guid>.Success(resultPost));

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<Guid>.Failure(ex.Message));
                    }
                }
            }
        }

        // PUT api/<PostController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, CreatePostRequest request)
        {
            var newPost = request.NewPost;
            var query = @"UPDATE Posts
                          SET In_Group = @In_Group, Post_Type_Id = @Post_Type_Id, Content = @Content, Destination_Id = @Destination_Id 
                          WHERE Id = @Id AND UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("In_Group", newPost.In_Group);
            parameters.Add("Destination_Id", newPost.Destination_Id);
            parameters.Add("Post_Type_Id", newPost.Post_Type_Id);
            parameters.Add("Content", newPost.Content);
            parameters.Add("UserId", request.UserId);
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return Ok(ResponseModel<string>.Success("Success."));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update Posts SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return Ok(ResponseModel<string>.Success("Success."));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }
    }
}
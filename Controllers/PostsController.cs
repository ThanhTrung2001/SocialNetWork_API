using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Models.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly DapperContext _context;

        public PostsController(DapperContext context)
        {
            _context = context;
        }

        // GET: api/<PostController>
        [HttpGet]
        public async Task<IEnumerable<PostQuery>> Get()
        {
            var query = @"
            SELECT 
                p.Id AS PostId,
                p.Content,
                p.PostTypeId,
                p.CreatedAt,
                p.InGroup,
                p.DestinationId,
                p.UserId,
                u.Email,
                ud.FirstName,
                ud.LastName,
                ud.Avatar,

                a.Id AS AttachmentId,
                a.Media,
                a.Description,

                s.Id AS SurveyId,
                s.ExpiredAt,
                s.Question,
                s.Total,

                si.Id AS SurveyItemId,
                si.OptionName AS SurveyItemName,
                si.Total AS ItemTotal,

                udv.UserId AS UserVoteId,
                udv.FirstName AS VoteFirstName,
                udv.LastName AS VoteLastName,
                udv.Avatar AS VoteAvatar,
                 
                c.Id AS CommentId,
                c.Content AS CommentContent,
                c.CreatedAt AS CommentCreatedAt,
                c.UserId AS CommentUserId,
                udc.FirstName AS CommentFirstName,
                udc.LastName AS CommentLastName,
                udc.Avatar AS CommentAvatar,

                r.Id AS ReactId,
                r.TypeName,
                udr.UserId AS ReactUserId,
                udr.FirstName AS ReactFirstName,
                udr.LastName AS ReactLastName,
                udr.Avatar AS ReactAvatar,
                udr.CreatedAt

            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.UserId = u.Id
            INNER JOIN 
                UserDetails ud ON u.Id = ud.UserId
            LEFT JOIN
                PostAttachment pa ON pa.PostId = p.Id
            LEFT JOIN
                Attachments a ON pa.AttachmentId = a.Id
            LEFT JOIN 
                Surveys s ON p.Id = s.PostId
            LEFT JOIN 
                SurveyItems si ON s.Id = si.SurveyId
            LEFT JOIN
                UserVote uv ON si.Id = uv.SurveyItemId
            LEFT JOIN 
                UserDetails udv ON udv.UserId = uv.UserId 
            LEFT JOIN
                Comments c ON p.Id = c.PostId
            LEFT JOIN
                UserDetails udc ON c.UserId = udc.UserId
            LEFT JOIN
                UserReactPost urp ON p.Id = urp.PostId
            LEFT JOIN 
                ReactTypes r ON r.Id = urp.ReactTypeId
            LEFT JOIN
                UserDetails udr ON urp.UserId = udr.UserId 
            WHERE 
                p.IsDeleted = 0;";

            try
            {
                var postDict = new Dictionary<Guid, PostQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, PostQuery>(
                    query,
                    map: (post, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!postDict.TryGetValue(post.PostId, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.PostId, postEntry);
                        }

                        if (post.PostTypeId == 1 && attachment != null && !postEntry.Attachments.Any((item) => item.AttachmentId == attachment.AttachmentId))
                        {
                            postEntry.Attachments.Add(attachment);
                        }

                        if (post.PostTypeId == 2 && survey != null)
                        {
                            postEntry.Survey = survey;
                            if (surveyItem != null && !postEntry.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                postEntry.Survey.SurveyItems.Add(surveyItem);
                                var result = postEntry.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.Votes.Any((item) => item.UserVoteId == vote.UserVoteId))
                                {
                                    result.Votes.Add(vote);
                                }
                            }
                        }

                        if (comment != null && !postEntry.Comments.Any((item) => item.CommentId == comment.CommentId))
                        {
                            postEntry.Comments.Add(comment);
                        }
                        if (react != null && !postEntry.Reacts.Any((item) => item.ReactId == react.ReactId))
                        {
                            postEntry.Reacts.Add(react);
                        }
                        return postEntry;
                    },

                    splitOn: "AttachmentId, SurveyId, SurveyItemId, UserVoteId, CommentId, ReactId");
                    return postDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("user/{id}")]
        public async Task<IEnumerable<PostQuery>> GetUserPostByUserID(Guid id)
        {
            var query = @"
            SELECT 
                p.Id AS PostId,
                p.Content,
                p.PostTypeId,
                p.CreatedAt,
                p.InGroup,
                p.DestinationId,
                p.UserId,
                u.Email,
                ud.FirstName,
                ud.LastName,
                ud.Avatar,

                a.Id AS AttachmentId,
                a.Media,
                a.Description,

                s.Id AS SurveyId,
                s.ExpiredAt,
                s.Question,
                s.Total,

                si.Id AS SurveyItemId,
                si.OptionName AS SurveyItemName,
                si.Total AS ItemTotal,

                udv.UserId AS UserVoteId,
                udv.FirstName AS VoteFirstName,
                udv.LastName AS VoteLastName,
                udv.Avatar AS VoteAvatar,
                 
                c.Id AS CommentId,
                c.Content AS CommentContent,
                c.CreatedAt AS CommentCreatedAt,
                c.UserId AS CommentUserId,
                udc.FirstName AS CommentFirstName,
                udc.LastName AS CommentLastName,
                udc.Avatar AS CommentAvatar,

                r.Id AS ReactId,
                r.TypeName,
                udr.UserId AS ReactUserId,
                udr.FirstName AS ReactFirstName,
                udr.LastName AS ReactLastName,
                udr.Avatar AS ReactAvatar,
                udr.CreatedAt

            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.UserId = u.Id
            INNER JOIN 
                UserDetails ud ON u.Id = ud.UserId
            LEFT JOIN
                PostAttachment pa ON pa.PostId = p.Id
            LEFT JOIN
                Attachments a ON pa.AttachmentId = a.Id
            LEFT JOIN 
                Surveys s ON p.Id = s.PostId
            LEFT JOIN 
                SurveyItems si ON s.Id = si.SurveyId
            LEFT JOIN
                UserVote uv ON si.Id = uv.SurveyItemId
            LEFT JOIN 
                UserDetails udv ON udv.UserId = uv.UserId 
            LEFT JOIN
                Comments c ON p.Id = c.PostId
            LEFT JOIN
                UserDetails udc ON c.UserId = udc.UserId
            LEFT JOIN
                UserReactPost urp ON p.Id = urp.PostId
            LEFT JOIN 
                ReactTypes r ON r.Id = urp.ReactTypeId
            LEFT JOIN
                UserDetails udr ON urp.UserId = udr.UserId 
            WHERE 
                p.IsDeleted = 0 AND p.UserId = @Id AND p.InGroup = 0;";

            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
                var postDict = new Dictionary<Guid, PostQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, PostQuery>(
                    query,
                    map: (post, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!postDict.TryGetValue(post.PostId, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.PostId, postEntry);
                        }

                        if (post.PostTypeId == 1 && attachment != null && !postEntry.Attachments.Any((item) => item.AttachmentId == attachment.AttachmentId))
                        {
                            postEntry.Attachments.Add(attachment);
                        }

                        if (post.PostTypeId == 2 && survey != null)
                        {
                            postEntry.Survey = survey;
                            if (surveyItem != null && !postEntry.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                postEntry.Survey.SurveyItems.Add(surveyItem);
                                var result = postEntry.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.Votes.Any((item) => item.UserVoteId == vote.UserVoteId))
                                {
                                    result.Votes.Add(vote);
                                }
                            }
                        }

                        if (comment != null && !postEntry.Comments.Any((item) => item.CommentId == comment.CommentId))
                        {
                            postEntry.Comments.Add(comment);
                        }
                        if (react != null && !postEntry.Reacts.Any((item) => item.ReactId == react.ReactId))
                        {
                            postEntry.Reacts.Add(react);
                        }
                        return postEntry;
                    },
                    parameter,
                    splitOn: "AttachmentId, SurveyId, SurveyItemId, UserVoteId, CommentId, ReactId");
                    return postDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public async Task<PostQuery> GetByID(Guid id)
        {
            var query = @"
            SELECT 
                p.Id AS PostId,
                p.Content,
                p.PostTypeId,
                p.CreatedAt,
                p.InGroup,
                p.DestinationId,
                p.UserId,
                u.Email,
                ud.FirstName,
                ud.LastName,
                ud.Avatar,

                a.Id AS AttachmentId,
                a.Media,
                a.Description,

                s.Id AS SurveyId,
                s.ExpiredAt,
                s.Question,
                s.Total,

                si.Id AS SurveyItemId,
                si.OptionName AS SurveyItemName,
                si.Total AS ItemTotal,

                udv.UserId AS UserVoteId,
                udv.FirstName AS VoteFirstName,
                udv.LastName AS VoteLastName,
                udv.Avatar AS VoteAvatar,
     
                c.Id AS CommentId,
                c.Content AS CommentContent,
                c.CreatedAt AS CommentCreatedAt,
                c.UserId AS CommentUserId,
                udc.FirstName AS CommentFirstName,
                udc.LastName AS CommentLastName,
                udc.Avatar AS CommentAvatar,

                r.Id AS ReactId,
                r.TypeName,
                udr.UserId AS ReactUserId,
                udr.FirstName AS ReactFirstName,
                udr.LastName AS ReactLastName,
                udr.Avatar AS ReactAvatar,
                udr.CreatedAt

            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.UserId = u.Id
            INNER JOIN 
                UserDetails ud ON u.Id = ud.UserId
            LEFT JOIN
                PostAttachment pa ON pa.PostId = p.Id
            LEFT JOIN
                Attachments a ON pa.AttachmentId = a.Id
            LEFT JOIN 
                Surveys s ON p.Id = s.PostId
            LEFT JOIN 
                SurveyItems si ON s.Id = si.SurveyId
            LEFT JOIN
                UserVote uv ON si.Id = uv.SurveyItemId
            LEFT JOIN 
                UserDetails udv ON udv.UserId = uv.UserId 
            LEFT JOIN
                Comments c ON p.Id = c.PostId
            LEFT JOIN
                UserDetails udc ON c.UserId = udc.UserId
            LEFT JOIN
                UserReactPost urp ON p.Id = urp.PostId
            LEFT JOIN 
                ReactTypes r ON r.Id = urp.ReactTypeId
            LEFT JOIN
                UserDetails udr ON urp.UserId = udr.UserId 
            WHERE 
                p.IsDeleted = 0 AND p.Id = @Id;";

            try
            {
                PostQuery postBasic = null;
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);


                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, PostQuery>(
                    query,
                    map: (post, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (postBasic == null)
                        {
                            postBasic = post;
                        }

                        if (post.PostTypeId == 1 && attachment != null && !postBasic.Attachments.Any((item) => item.AttachmentId == attachment.AttachmentId))
                        {
                            postBasic.Attachments.Add(attachment);
                        }

                        if (post.PostTypeId == 2 && survey != null)
                        {
                            postBasic.Survey = survey;
                            if (surveyItem != null && !postBasic.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                postBasic.Survey.SurveyItems.Add(surveyItem);
                                var result = postBasic.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.Votes.Any((item) => item.UserVoteId == vote.UserVoteId))
                                {
                                    result.Votes.Add(vote);
                                }
                            }
                        }

                        if (comment != null && !postBasic.Comments.Any((item) => item.CommentId == comment.CommentId))
                        {
                            postBasic.Comments.Add(comment);
                        }
                        if (react != null && !postBasic.Reacts.Any((item) => item.ReactId == react.ReactId))
                        {
                            postBasic.Reacts.Add(react);
                        }
                        return postBasic;
                    },
                    parameter,
                    splitOn: "AttachmentId, SurveyId, SurveyItemId, UserVoteId, CommentId, ReactId");
                    return postBasic;
                }
            }
            catch
            {
                throw;
            }
        }

        // POST api/<PostController>
        [HttpPost]
        public async Task<IActionResult> Post(CreatePostRequest request)
        {
            var newPost = request.NewPost;
            var queryPost = @"INSERT INTO Posts (Id, CreatedAt, UpdatedAt, IsDeleted, DestinationId, InGroup, PostTypeId, Content, UserId, ReactCount)
                              OUTPUT Inserted.Id
                              VALUES 
                              (NEWID(), GETDATE(), GETDATE(), 0, @DestinationId, @InGroup, @PostTypeId, @Content, @UserId, 0);";
            var queryAttachment = @"INSERT INTO Attachments (Id, CreatedAt, UpdatedAt, IsDeleted, Media, Description)
                               OUTPUT Inserted.Id
                               VALUES 
                               (NEWID(), GETDATE(), GETDATE(), 0, @Media, @Description)";
            var queryPostAttachment = @"INSERT INTO PostAttachment (PostId, AttachmentId)
                               VALUES 
                               (@PostId, @AttachmentId)";
            var querySurvey = @"INSERT INTO Surveys (ID, CreatedAt, UpdatedAt, IsDeleted, ExpiredAt, Total, SurveyTypeId, PostId, Question)
                                OUTPUT Inserted.Id
                                VALUES
                                (NEWID(), GETDATE(), GETDATE(), 0, @ExpiredAt, 0, @SurveyTypeId, @PostId, @Question)";
            var querySurveyItem = @"INSERT INTO SurveyItems (ID, CreatedAt, UpdatedAt, IsDeleted, OptionName, Total, SurveyId)
                                    OUTPUT Inserted.Id
                                    VALUES
                                    (NEWID(), GETDATE(), GETDATE(), 0, @OptionName, 0, @SurveyId)";

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("InGroup", newPost.InGroup);
                        parameters.Add("DestinationId", newPost.DestinationId);
                        parameters.Add("PostTypeId", newPost.PostTypeId);
                        parameters.Add("Content", newPost.Content);
                        parameters.Add("UserId", request.UserId);
                        //insert post first
                        var resultPost = await connection.QuerySingleAsync<Guid>(queryPost, parameters, transaction);
                        if (request.NewPost.PostTypeId == 1)
                        {
                            foreach (var item in request.NewPost.Attachments)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("PostId", resultPost);
                                parameters.Add("Media", item.Media);
                                parameters.Add("Description", item.Description);
                                var resultAttachment = await connection.QuerySingleAsync<Guid>(queryAttachment, parameters, transaction);
                                parameters.Add("AttachmentId", resultAttachment);
                                await connection.ExecuteAsync(queryPostAttachment, parameters, transaction);
                            }
                        }
                        else if (request.NewPost.PostTypeId == 2)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("PostId", resultPost);
                            parameters.Add("ExpiredAt", request.NewPost.Survey.ExpiredAt);
                            parameters.Add("Question", request.NewPost.Survey.Question);
                            parameters.Add("SurveyTypeId", request.NewPost.Survey.SurveyTypeId);
                            var resultSurvey = await connection.QuerySingleAsync<Guid>(querySurvey, parameters, transaction);
                            foreach (var item in request.NewPost.Survey.SurveyItems)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("SurveyId", resultSurvey);
                                parameters.Add("OptionName", item.OptionName);
                                await connection.ExecuteAsync(querySurveyItem, parameters, transaction);
                            }
                        }
                        transaction.Commit();
                        return Ok();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ex.Message);
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
                          SET InGroup = @InGroup, PostTypIde = @PostTypeId, Content = @Content, DestinationId = @DestinationId 
                          WHERE Id = @Id AND UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("InGroup", newPost.InGroup);
            parameters.Add("DestinationId", newPost.DestinationId);
            parameters.Add("PostTypeId", newPost.PostTypeId);
            parameters.Add("Content", newPost.Content);
            parameters.Add("UserId", request.UserId);
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update Posts SET isDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }
    }
}
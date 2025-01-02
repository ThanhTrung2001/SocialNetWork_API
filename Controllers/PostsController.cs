using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
        public async Task<IEnumerable<PostBasicQuery>> Get()
        {
            var query = @"
            SELECT 
                p.Id AS PostId,
                p.Content AS PostContent,
                p.PostType,
                p.CreatedAt,
                p.PostDestination,
                u.Id AS UserId,
                u.UserName,
                u.Email,
                u.AvatarUrl,

                m.URL AS MediaUrl,

                s.Id AS SurveyId,
                s.ExpiredIn,
                s.Question AS SurveyQuestion,

                si.Id AS SurveyItemId,
                si.Content AS SurveyItemContent,
                si.Votes AS SurveyItemVotes,

                sv.VoteId,
                sv.UserId AS VoteUserId,              
                usv.UserName AS VoteUserName,
                usv.AvatarUrl AS VoteUserAvatar,
                 
                c.Id AS CommentId,
                c.Content AS CommentContent,
                c.MediaURL AS CommentMediaUrl,
                c.CreatedAt AS CommentCreatedAt,
                c.UserId AS CommentUserId,
                uc.UserName AS CommentUserName,
                uc.AvatarUrl AS CommentUserAvatarUrl,

                r.Id AS ReactId,
                r.ReactType,
                ur.Id AS ReactUserId,
                ur.UserName AS ReactUserName,
                ur.AvatarUrl AS ReactUserAvatar
            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.OwnerId = u.Id
            LEFT JOIN
                MediaItems m ON p.Id = m.PostId
            LEFT JOIN 
                Surveys s ON p.Id = s.PostId
            LEFT JOIN 
                SurveyItems si ON s.Id = si.SurveyId
            LEFT JOIN
                SurveyVotes sv ON si.Id = sv.OptionId
            LEFT JOIN 
                Users usv ON usv.Id = sv.UserId 
            LEFT JOIN
                Comments c ON p.Id = c.PostId
            LEFT JOIN
                Users uc ON c.UserId = uc.Id
            LEFT JOIN 
                Reacts r ON p.Id = r.PostId
            LEFT JOIN
                Users ur ON r.UserId = ur.Id 
            WHERE 
                p.IsDeleted = 0;";

            try
            {
                var postDict = new Dictionary<Guid, PostBasicQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostBasicQuery, string, PostSurveyQuery, SurveyItemQuery, SurveyItemVote, PostCommentQuery, PostReactQuery, PostBasicQuery>(
                    query,
                    map: (post, mediaUrl, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!postDict.TryGetValue(post.PostId, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.PostId, postEntry);
                        }

                        if (post.PostType == "media" && !string.IsNullOrEmpty(mediaUrl) && !postEntry.MediaUrls.Any((item) => item == mediaUrl))
                        {
                            postEntry.MediaUrls.Add(mediaUrl);
                        }

                        if (post.PostType == "survey" && survey != null)
                        {
                            postEntry.Survey = survey;
                            if (surveyItem != null && !postEntry.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                postEntry.Survey.SurveyItems.Add(surveyItem);
                                var result = postEntry.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.SurveyVotes.Any((item) => item.VoteId == vote.VoteId))
                                {
                                    result.SurveyVotes.Add(vote);
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

                    splitOn: "MediaUrl, SurveyId, SurveyItemId, VoteId, CommentId, ReactId");
                    return postDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("user/{id}")]
        public async Task<IEnumerable<PostBasicQuery>> GetUserPostByUserID(Guid id)
        {
            var query = @"
            SELECT 
                p.Id AS PostId,
                p.Content AS PostContent,
                p.PostType,
                p.CreatedAt,
                p.PostDestination,
                u.Id AS UserId,
                u.UserName,
                u.Email,
                u.AvatarUrl,

                m.URL AS MediaUrl,

                s.Id AS SurveyId,
                s.ExpiredIn,
                s.Question AS SurveyQuestion,

                si.Id AS SurveyItemId,
                si.Content AS SurveyItemContent,
                si.Votes AS SurveyItemVotes,

                sv.VoteId,
                sv.UserId AS VoteUserId,              
                usv.UserName AS VoteUserName,
                usv.AvatarUrl AS VoteUserAvatar,
                 
                c.Id AS CommentId,
                c.Content AS CommentContent,
                c.MediaURL AS CommentMediaUrl,
                c.CreatedAt AS CommentCreatedAt,
                c.UserId AS CommentUserId,
                uc.UserName AS CommentUserName,
                uc.AvatarUrl AS CommentUserAvatarUrl,

                r.Id AS ReactId,
                r.ReactType,
                ur.Id AS ReactUserId,
                ur.UserName AS ReactUserName,
                ur.AvatarUrl AS ReactUserAvatar
            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.OwnerId = u.Id
            LEFT JOIN
                MediaItems m ON p.Id = m.PostId
            LEFT JOIN 
                Surveys s ON p.Id = s.PostId
            LEFT JOIN 
                SurveyItems si ON s.Id = si.SurveyId
            LEFT JOIN
                SurveyVotes sv ON si.Id = sv.OptionId
            LEFT JOIN 
                Users usv ON usv.Id = sv.UserId 
            LEFT JOIN
                Comments c ON p.Id = c.PostId
            LEFT JOIN
                Users uc ON c.UserId = uc.Id
            LEFT JOIN 
                Reacts r ON p.Id = r.PostId
            LEFT JOIN
                Users ur ON r.UserId = ur.Id 
            WHERE 
                p.IsDeleted = 0 AND p.OwnerId = @Id AND p.PostType = 'personal';";

            try
            {
                var postDict = new Dictionary<Guid, PostBasicQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostBasicQuery, string, PostSurveyQuery, SurveyItemQuery, SurveyItemVote, PostCommentQuery, PostReactQuery, PostBasicQuery>(
                    query,
                    map: (post, mediaUrl, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!postDict.TryGetValue(post.PostId, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.PostId, postEntry);
                        }

                        if (post.PostType == "media" && !string.IsNullOrEmpty(mediaUrl) && !postEntry.MediaUrls.Any((item) => item == mediaUrl))
                        {
                            postEntry.MediaUrls.Add(mediaUrl);
                        }

                        if (post.PostType == "survey" && survey != null)
                        {
                            postEntry.Survey = survey;
                            if (surveyItem != null && !postEntry.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                postEntry.Survey.SurveyItems.Add(surveyItem);
                                var result = postEntry.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.SurveyVotes.Any((item) => item.VoteId == vote.VoteId))
                                {
                                    result.SurveyVotes.Add(vote);
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

                    splitOn: "MediaUrl, SurveyId, SurveyItemId, VoteId, CommentId, ReactId");
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
        public async Task<PostBasicQuery> GetByID(Guid id)
        {
            var query = @"
            SELECT 
                p.Id AS PostId,
                p.Content AS PostContent,
                p.PostType,
                p.CreatedAt,
                p.PostDestination,
                u.Id AS UserId,
                u.UserName,
                u.Email,
                u.AvatarUrl,

                m.URL AS MediaUrl,

                s.Id AS SurveyId,
                s.ExpiredIn,
                s.Question AS SurveyQuestion,

                si.Id AS SurveyItemId,
                si.Content AS SurveyItemContent,
                si.Votes AS SurveyItemVotes,

                sv.VoteId,
                sv.UserId AS VoteUserId,              
                usv.UserName AS VoteUserName,
                usv.AvatarUrl AS VoteUserAvatar,
                 
                c.Id AS CommentId,
                c.Content AS CommentContent,
                c.MediaURL AS CommentMediaUrl,
                c.CreatedAt AS CommentCreatedAt,
                c.UserId AS CommentUserId,
                uc.UserName AS CommentUserName,
                uc.AvatarUrl AS CommentUserAvatarUrl,

                r.Id AS ReactId,
                r.ReactType,
                ur.Id AS ReactUserId,
                ur.UserName AS ReactUserName,
                ur.AvatarUrl AS ReactUserAvatar
            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.OwnerId = u.Id
            LEFT JOIN
                MediaItems m ON p.Id = m.PostId
            LEFT JOIN 
                Surveys s ON p.Id = s.PostId
            LEFT JOIN 
                SurveyItems si ON s.Id = si.SurveyId
            LEFT JOIN
                SurveyVotes sv ON si.Id = sv.OptionId
            LEFT JOIN 
                Users usv ON usv.Id = sv.UserId 
            LEFT JOIN
                Comments c ON p.Id = c.PostId
            LEFT JOIN
                Users uc ON c.UserId = uc.Id
            LEFT JOIN 
                Reacts r ON p.Id = r.PostId
            LEFT JOIN
                Users ur ON r.UserId = ur.Id 
            WHERE 
                p.IsDeleted = 0 AND p.Id = @Id;";

            try
            {
                PostBasicQuery postBasic = null;

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostBasicQuery, string, PostSurveyQuery, SurveyItemQuery, SurveyItemVote, PostCommentQuery, PostReactQuery, PostBasicQuery>(
                    query,
                    map: (post, mediaUrl, survey, surveyItem, vote, comment, react) =>
                    {
                        if (postBasic == null)
                        {
                            postBasic = post;
                        }

                        if (post.PostType == "media" && !string.IsNullOrEmpty(mediaUrl) && !postBasic.MediaUrls.Any((item) => item == mediaUrl))
                        {
                            postBasic.MediaUrls.Add(mediaUrl);
                        }

                        if (post.PostType == "survey" && survey != null)
                        {
                            postBasic.Survey = survey;
                            if (surveyItem != null && !postBasic.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                postBasic.Survey.SurveyItems.Add(surveyItem);
                                var result = postBasic.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.SurveyVotes.Any((item) => item.VoteId == vote.VoteId))
                                {
                                    result.SurveyVotes.Add(vote);
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
                    new { Id = id },
                    splitOn: "MediaUrl, SurveyId, SurveyItemId, VoteId, CommentId, ReactId");
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
            var userId = Guid.Parse(request.UserId);
            var newPost = request.NewPost;
            var queryPost = @"INSERT INTO Posts (Id, CreatedAt, UpdatedAt, IsDeleted, DestinationId, IsNotification, PostType, PostDestination, Content, OwnerId)
                              OUTPUT Inserted.Id
                              VALUES 
                              (NEWID(), GETDATE(), GETDATE(), 0, 'dest1', @IsNotification, @PostType, 'All', @Content, @UserId);";
            var queryMedia = @"INSERT INTO MediaItems (Id, CreatedAt, UpdatedAt, IsDeleted, FileType, URL, PostId)
                               VALUES 
                               (NEWID(), GETDATE(), GETDATE(), 0, 3, @URL, @PostId)";
            var querySurvey = @"INSERT INTO Surveys (ID, CreatedAt, UpdatedAt, IsDeleted, ExpiredIn, PostId, Question)
                                OUTPUT Inserted.Id
                                VALUES
                                (NEWID(), GETDATE(), GETDATE(), 0, @ExpiredIn, @PostId, @Question)";
            var querySurveyItem = @"INSERT INTO SurveyItems (ID, CreatedAt, UpdatedAt, IsDeleted, Content, SurveyId, Votes)
                                    OUTPUT Inserted.Id
                                    VALUES
                                    (NEWID(), GETDATE(), GETDATE(), 0, @Content, @SurveyId, 0)";

            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("IsNotification", newPost.IsNotification, DbType.Boolean);
                        parameters.Add("PostType", newPost.PostType, DbType.String);
                        parameters.Add("PostDestination", newPost.PostDestination, DbType.String);
                        parameters.Add("Content", newPost.Content, DbType.String);
                        parameters.Add("UserId", userId, DbType.Guid);
                        //insert post first
                        var resultPost = await connection.QuerySingleAsync<Guid>(queryPost, parameters, transaction);
                        if (request.NewPost.PostType == "media")
                        {
                            foreach (var item in request.NewPost.MediaUrls)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("PostId", resultPost);
                                parameters.Add("URL", item);
                                await connection.ExecuteAsync(queryMedia, parameters, transaction);
                            }
                        }
                        else if (request.NewPost.PostType == "survey")
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("PostId", resultPost);
                            parameters.Add("ExpiredIn", request.NewPost.Survey.ExpiredIn);
                            parameters.Add("Question", request.NewPost.Survey.Question);
                            var resultSurvey = await connection.QuerySingleAsync<Guid>(querySurvey, parameters, transaction);
                            foreach (var item in request.NewPost.Survey.SurveyItems)
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("SurveyId", resultSurvey);
                                parameters.Add("Content", item.Content);
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
        public async Task<IActionResult> Put(Guid id, NewPostBasic edit)
        {
            var query = "UPDATE Posts SET IsNotification = @IsNotification, PostType = @PostType, Content = @Content, PostDestination = @PostDestination WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("IsNotification", edit.IsNotification, DbType.Boolean);
            parameters.Add("PostType", edit.PostType, DbType.String);
            parameters.Add("PostDestination", edit.PostDestination, DbType.String);
            parameters.Add("Content", edit.Content, DbType.String);
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
            var query = "Update Posts SET isDeleted = 1 WHERE Id = @Id";
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
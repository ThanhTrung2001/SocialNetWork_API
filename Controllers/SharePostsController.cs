using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Models.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SharePostsController : ControllerBase
    {
        private readonly DapperContext _context;

        public SharePostsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<SharePostQuery>> Get()
        {
            var query = @"SELECT 
                            sh.Id,
                            sh.Content AS ShareContent,
                            sh.CreatedAt AS ShareCreatedAt,
                            sh.SharedByUserId,
                            sh.InGroup AS ShareIngroup,
                            uds.FirstName AS ShareFirstName,
                            uds.LastName AS ShareLastName,
                            uds.Avatar AS ShareAvatar,
                            
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
                            udv.Avatar AS VoteAvatar

                         FROM SharePosts sh
                         INNER JOIN 
                            UserDetails uds ON sh.SharedByUserId = uds.UserID
                         LEFT JOIN 
                            Posts p ON p.Id = sh.SharedPostId
                         LEFT JOIN 
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
                        WHERE sh.IsDeleted = 0";
            try
            {
                var shareDict = new Dictionary<Guid, SharePostQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<SharePostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, SharePostQuery>(
                    query,
                    map: (share, attachment, survey, surveyItem, vote) =>
                    {
                        if (!shareDict.TryGetValue(share.Id, out var shareEntry))
                        {
                            shareEntry = share;
                            shareDict.Add(share.Id, shareEntry);
                        }

                        if (share.PostTypeId == 1 && attachment != null && !shareEntry.Attachments.Any((item) => item == attachment))
                        {
                            shareEntry.Attachments.Add(attachment);
                        }

                        if (share.PostTypeId == 2 && survey != null)
                        {
                            share.Survey = survey;
                            if (surveyItem != null && !share.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                share.Survey.SurveyItems.Add(surveyItem);
                                var result = share.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.Votes.Any((item) => item.UserVoteId == vote.UserVoteId))
                                {
                                    result.Votes.Add(vote);
                                }
                            }
                        }

                        //if (comment != null && !shareEntry.Comments.Any((item) => item.CommentId == comment.CommentId))
                        //{
                        //    shareEntry.Comments.Add(comment);
                        //}
                        //if (react != null && !shareEntry.Reacts.Any((item) => item.ReactId == react.ReactId))
                        //{
                        //    shareEntry.Reacts.Add(react);
                        //}
                        return shareEntry;
                    },

                    splitOn: "AttachmentId, SurveyId, SurveyItemId, UserVoteId");
                    return shareDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        //[HttpGet("{id}")]
        //public async Task<SharePostQuery> GetbyId(Guid id)
        //{

        //}

        [HttpGet("user/{id}")]
        public async Task<IEnumerable<SharePostQuery>> GetByUserID(Guid id)
        {
            var query = @"SELECT 
                            sh.Id,
                            sh.Content AS ShareContent,
                            sh.CreatedAt AS ShareCreatedAt,
                            sh.SharedByUserId,
                            sh.InGroup AS ShareIngroup,
                            uds.FirstName AS ShareFirstName,
                            uds.LastName AS ShareLastName,
                            uds.Avatar AS ShareAvatar,
                            
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
                            udv.Avatar AS VoteAvatar

                         FROM SharePosts sh
                         INNER JOIN 
                            UserDetails uds ON sh.SharedByUserId = uds.UserID
                         LEFT JOIN 
                            Posts p ON p.Id = sh.SharedPostId
                         LEFT JOIN 
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
                        WHERE sh.IsDeleted = 0 AND sh.SharedByUserId = @Id";
            try
            {
                var shareDict = new Dictionary<Guid, SharePostQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<SharePostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, SharePostQuery>(
                    query,
                    map: (share, attachment, survey, surveyItem, vote) =>
                    {
                        if (!shareDict.TryGetValue(share.Id, out var shareEntry))
                        {
                            shareEntry = share;
                            shareDict.Add(share.Id, shareEntry);
                        }

                        if (share.PostTypeId == 1 && attachment != null && !shareEntry.Attachments.Any((item) => item == attachment))
                        {
                            shareEntry.Attachments.Add(attachment);
                        }

                        if (share.PostTypeId == 2 && survey != null)
                        {
                            share.Survey = survey;
                            if (surveyItem != null && !share.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                share.Survey.SurveyItems.Add(surveyItem);
                                var result = share.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.Votes.Any((item) => item.UserVoteId == vote.UserVoteId))
                                {
                                    result.Votes.Add(vote);
                                }
                            }
                        }

                        //if (comment != null && !shareEntry.Comments.Any((item) => item.CommentId == comment.CommentId))
                        //{
                        //    shareEntry.Comments.Add(comment);
                        //}
                        //if (react != null && !shareEntry.Reacts.Any((item) => item.ReactId == react.ReactId))
                        //{
                        //    shareEntry.Reacts.Add(react);
                        //}
                        return shareEntry;
                    },

                    splitOn: "AttachmentId, SurveyId, SurveyItemId, UserVoteId");
                    return shareDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("post/{postId}/users")]
        public async Task<IEnumerable<ShareUserQuery>> GetShareUsersByPostId(Guid postId)
        {
            var query = @" SELECT s.Id, s.SharedByUserId AS ShareUserId, ud.FirstName AS ShareFirstName, ud.LastName AS ShareLastName, u.Avatar as ShareAvatar
                           FROM SharePosts s
                           LEFT JOIN UserDetails ud ON s.ShareByUserId = ud.UserId
                           WHERE s.SharePostId = @PostId AND s.IsDeleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("PostId", postId);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ShareUserQuery>(query, parameter);
                return result;
            }
        }

        [HttpPost("post/{postId}")]
        public async Task<IActionResult> CreateSharePost(Guid postId, CreateSharePostCommand share)
        {
            var query = @"INSERT INTO SharePosts (Id, CreatedAt, UpdatedAt, IsDeleted, SharedPostId ,SharedByUserId, Content, InGroup, DestinationId)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @PostId, @SharedByUserId, @Content, @InGroup, @DestinationId)";
            var parameters = new DynamicParameters();
            parameters.Add("PostId", postId);
            parameters.Add("SharedByUserId", share.SharedByUserId);
            parameters.Add("Content", share.Content);
            parameters.Add("InGroup", share.InGroup);
            parameters.Add("DestinationId", share.DestinationId);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> EditSharePost(Guid id, EditSharePostCommand command)
        {
            var query = @"UPDATE SharePosts SET Content = @Content WHERE Id = @Id AND SharedByUserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("UserId", command.SharedByUserId);
            parameters.Add("Content", command.ShareContent);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "UPDATE SharePosts SET isDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id;";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }
    }
}
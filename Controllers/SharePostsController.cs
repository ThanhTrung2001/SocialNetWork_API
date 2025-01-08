using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class Share_PostsController : ControllerBase
    {
        private readonly DapperContext _context;

        public Share_PostsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<SharePostQuery>> Get()
        {
            var query = @"SELECT 
                            sh.Id,
                            sh.Content AS Share_Content,
                            sh.Created_At AS Share_Created_At,
                            sh.Shared_By_User_Id,
                            sh.In_Group AS Share_In_Group,
                            uds.FirstName AS Share_FirstName,
                            uds.LastName AS Share_LastName,
                            uds.Avatar AS Share_Avatar,
                            
                            p.Id AS PostId,
                            p.Content,
                            p.Post_Type_Id,
                            p.Created_At,
                            p.In_Group,
                            p.Destination_Id,
                            p.User_Id,
                            u.Email,
                            ud.FirstName,
                            ud.LastName,
                            ud.Avatar,

                            a.Id AS Attachment_Id,
                            a.Media,
                            a.Description,

                            s.Id AS Survey_Id,
                            s.Expired_At,
                            s.Question,
                            s.Survey_Type,
                            s.Total_Vote,

                            si.Id AS SurveyItem_Id,
                            si.Option_Name AS SurveyItem_Name,
                            si.Total_Vote AS Item_Total,

                            udv.User_Id AS Vote_UserId,
                            udv.FirstName AS Vote_FirstName,
                            udv.LastName AS Vote_LastName,
                            udv.Avatar AS Vote_Avatar,

                            c.Id AS Comment_Id,
                            c.Content AS Comment_Content,
                            c.Created_At AS Comment_Created_At,
                            c.User_Id AS Comment_UserId,
                            udc.FirstName AS Comment_FirstName,
                            udc.LastName AS Comment_LastName,
                            udc.Avatar AS Comment_Avatar,

                            urp.React_Type,
                            udr.User_Id AS React_UserId,
                            udr.FirstName AS React_FirstName,
                            udr.LastName AS React_LastName,
                            udr.Avatar AS React_Avatar,
                            udr.Created_At

                         FROM Share_Posts sh
                         INNER JOIN 
                            User_Details uds ON sh.Shared_By_User_Id = uds.User_Id
                         LEFT JOIN 
                            Posts p ON p.Id = sh.Shared_Post_Id
                         LEFT JOIN 
                            Users u ON p.User_Id = u.Id
                        LEFT JOIN 
                            User_Details ud ON u.Id = ud.User_Id
                        LEFT JOIN
                            Post_Attachment pa ON pa.Post_Id = p.Id
                        LEFT JOIN
                            Attachments a ON pa.Attachment_Id = a.Id
                        LEFT JOIN 
                            Surveys s ON p.Id = s.Id
                        LEFT JOIN 
                            Survey_Items si ON s.Id = si.Survey_Id
                        LEFT JOIN
                            User_SurveyItem_Vote uv ON si.Id = uv.SurveyItem_Id
                        LEFT JOIN 
                            User_Details udv ON udv.User_Id = uv.User_Id
                        LEFT JOIN
                            Comments c ON sh.Id = c.Post_Id
                        LEFT JOIN
                            User_Details udc ON c.User_Id = udc.User_Id
                        LEFT JOIN
                            User_React_Post urp ON sh.Id = urp.Post_Id
                        LEFT JOIN
                            User_Details udr ON urp.User_Id = udr.User_Id
                        WHERE 
                            sh.Is_Deleted = 0";
            try
            {
                var shareDict = new Dictionary<Guid, SharePostQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<SharePostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, SharePostQuery>(
                    query,
                    map: (share, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!shareDict.TryGetValue(share.Id, out var shareEntry))
                        {
                            shareEntry = share;
                            shareDict.Add(share.Id, shareEntry);
                        }

                        if (share.Post_Type_Id == 1 && attachment != null && !shareEntry.Attachments.Any((item) => item == attachment))
                        {
                            shareEntry.Attachments.Add(attachment);
                        }

                        if (share.Post_Type_Id == 2 && survey != null)
                        {
                            share.Survey = survey;
                            if (surveyItem != null && !share.Survey.SurveyItems.Any((item) => item.SurveyItem_Id == surveyItem.SurveyItem_Id))
                            {
                                share.Survey.SurveyItems.Add(surveyItem);
                                var result = share.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItem_Id == surveyItem.SurveyItem_Id);
                                if (vote != null && !result.Votes.Any((item) => item.Vote_UserId == vote.Vote_UserId))
                                {
                                    result.Votes.Add(vote);
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

                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
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
                            sh.Content AS Share_Content,
                            sh.Created_At AS Share_Created_At,
                            sh.Shared_By_User_Id,
                            sh.In_Group AS Share_In_Group,
                            uds.FirstName AS Share_FirstName,
                            uds.LastName AS Share_LastName,
                            uds.Avatar AS Share_Avatar,
                            
                            p.Id AS Post_Id,
                            p.Content,
                            p.Post_Type_Id,
                            p.Created_At,
                            p.In_Group,
                            p.Destination_Id,
                            p.User_Id,
                            u.Email,
                            ud.FirstName,
                            ud.LastName,
                            ud.Avatar,

                            a.Id AS Attachment_Id,
                            a.Media,
                            a.Description,

                            s.Id AS Survey_Id,
                            s.Expired_At,
                            s.Question,
                            s.Survey_Type,
                            s.Total_Vote,

                            si.Id AS SurveyItem_Id,
                            si.Option_Name AS SurveyItem_Name,
                            si.Total_Vote AS Item_Total,

                            udv.User_Id AS Vote_UserId,
                            udv.FirstName AS Vote_FirstName,
                            udv.LastName AS Vote_LastName,
                            udv.Avatar AS Vote_Avatar,

                         FROM Share_Posts sh
                         INNER JOIN 
                            User_Details uds ON sh.Shared_By_User_Id = uds.User_Id
                         LEFT JOIN 
                            Posts p ON p.Id = sh.Shared_Post_Id
                         LEFT JOIN 
                            Users u ON p.User_Id = u.Id
                         INNER JOIN 
                            User_Details ud ON u.Id = ud.User_Id
                         LEFT JOIN
                            Post_Attachment pa ON pa.Post_Id = p.Id
                         LEFT JOIN
                            Attachments a ON pa.Attachment_Id = a.Id
                         LEFT JOIN 
                            Surveys s ON p.Id = s.Id
                         LEFT JOIN 
                            Survey_Items si ON s.Id = si.Survey_Id
                         LEFT JOIN
                            User_SurveyItem_Vote uv ON si.Id = uv.SurveyItem_Id
                         LEFT JOIN 
                            User_Details udv ON udv.User_Id = uv.User_Id 
                         LEFT JOIN
                            Comments c ON p.Id = c.Post_Id
                         LEFT JOIN
                            User_Details udc ON c.User_Id = udc.User_Id
                         LEFT JOIN
                            User_React_Post urp ON p.Id = urp.Post_Id
                         LEFT JOIN
                            User_Details udr ON urp.User_Id = udr.User_Id 
                         WHERE sh.Is_Deleted = 0 AND sh.Shared_By_User_Id = @Id";
            try
            {
                var shareDict = new Dictionary<Guid, SharePostQuery>();
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<SharePostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, SharePostQuery>(
                    query,
                    map: (share, attachment, survey, surveyItem, vote, comment, react) =>
                    {
                        if (!shareDict.TryGetValue(share.Id, out var shareEntry))
                        {
                            shareEntry = share;
                            shareDict.Add(share.Id, shareEntry);
                        }

                        if (share.Post_Type_Id == 1 && attachment != null && !shareEntry.Attachments.Any((item) => item == attachment))
                        {
                            shareEntry.Attachments.Add(attachment);
                        }

                        if (share.Post_Type_Id == 2 && survey != null)
                        {
                            share.Survey = survey;
                            if (surveyItem != null && !share.Survey.SurveyItems.Any((item) => item.SurveyItem_Id == surveyItem.SurveyItem_Id))
                            {
                                share.Survey.SurveyItems.Add(surveyItem);
                                var result = share.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItem_Id == surveyItem.SurveyItem_Id);
                                if (vote != null && !result.Votes.Any((item) => item.Vote_UserId == vote.Vote_UserId))
                                {
                                    result.Votes.Add(vote);
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
                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
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
            var query = @" SELECT s.Id, s.Shared_By_User_Id AS Share_UserId, ud.FirstName AS Share_FirstName, ud.LastName AS Share_LastName, u.Avatar as Share_Avatar
                           FROM Share_Posts s
                           LEFT JOIN User_Details ud ON s.Shared_By_User_Id = ud.User_Id
                           WHERE s.Shared_Post_Id = @PostId AND s.Is_Deleted = 0";
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
            var query = @"INSERT INTO Share_Posts (Id, Created_At, Updated_At, Is_Deleted, Shared_Post_Id ,Shared_By_User_Id, Content, In_Group, Destination_Id)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @PostId, @Shared_By_User_Id, @Content, @In_Group, @Destination_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("PostId", postId);
            parameters.Add("Shared_By_User_Id", share.Shared_By_User_Id);
            parameters.Add("Content", share.Content);
            parameters.Add("In_Group", share.In_Group);
            parameters.Add("Destination_Id", share.Destination_Id);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> EditSharePost(Guid id, EditSharePostCommand command)
        {
            var query = @"UPDATE Share_Posts SET Content = @Content WHERE Id = @Id AND Shared_By_User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("User_Id", command.Shared_By_User_Id);
            parameters.Add("Content", command.Share_Content);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "UPDATE Share_Posts SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id;";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }
    }
}
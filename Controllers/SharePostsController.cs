using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
using EnVietSocialNetWorkAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
        public async Task<IActionResult> Get()
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
                    commandType: CommandType.StoredProcedure,
                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
                    return Ok(ResponseModel<IEnumerable<SharePostQuery>>.Success(shareDict.Values.ToList()));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<SharePostQuery>>.Failure(ex.Message));
            }
        }

        //[HttpGet("{id}")]
        //public async Task<SharePostQuery> GetbyId(Guid id)
        //{

        //}

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetByUserID(Guid id)
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
                    commandType: CommandType.StoredProcedure,
                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
                    return Ok(ResponseModel<IEnumerable<SharePostQuery>>.Success(shareDict.Values.ToList()));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<SharePostQuery>>.Failure(ex.Message));
            }
        }

        [HttpGet("post/{Post_Id}/users")]
        public async Task<IActionResult> GetShareUsersByPost_Id(Guid Post_Id)
        {
            var query = @" SELECT s.Id, s.Shared_By_User_Id AS Share_UserId, ud.FirstName AS Share_FirstName, ud.LastName AS Share_LastName, u.Avatar as Share_Avatar
                           FROM Share_Posts s
                           LEFT JOIN User_Details ud ON s.Shared_By_User_Id = ud.User_Id
                           WHERE s.Shared_Post_Id = @Post_Id AND s.Is_Deleted = 0";
            var parameter = new DynamicParameters();
            parameter.Add("Post_Id", Post_Id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<ShareUserQuery>(query, parameter);
                    return Ok(ResponseModel<IEnumerable<ShareUserQuery>>.Success(result));
                }

                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<ShareUserQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSharePost(CreateSharePostCommand share)
        {
            var query = @"INSERT INTO Share_Posts (Id, Created_At, Updated_At, Is_Deleted, Shared_Post_Id ,Shared_By_User_Id, Content, In_Group, Destination_Id)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Post_Id, @Shared_By_User_Id, @Content, @In_Group, @Destination_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("Post_Id", share.Post_Id);
            parameters.Add("Shared_By_User_Id", share.Shared_By_User_Id);
            parameters.Add("Content", share.Content);
            parameters.Add("In_Group", share.In_Group);
            parameters.Add("Destination_Id", share.Destination_Id);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QuerySingleAsync(query, parameters);
                    return Ok(ResponseModel<Guid>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<Guid>.Failure(ex.Message));
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSharePost(Guid id, EditSharePostCommand command)
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
                    return Ok(ResponseModel<string>.Success("Success."));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<string>.Failure(ex.Message));
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "UPDATE Share_Posts SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameter);
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
using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Queries;
using EnVietSocialNetWorkAPI.Models.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly DapperContext _context;
        public PagesController(DapperContext context) { _context = context; }

        [HttpGet]
        public async Task<IEnumerable<PageQuery>> Get()
        {
            var query = @"SELECT p.Id, p.Name, p.Avatar, p.Wallpaper, p.Created_At
                          FROM Pages p
                          WHERE p.Is_Deleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<PageQuery>(query);
                return result;
            }
        }

        [HttpGet("search")]
        public async Task<IEnumerable<PageQuery>> Search([FromQuery] string name)
        {
            var query = @"SELECT p.Id, p.Name, p.Avatar, p.Wallpaper, p.Created_At
                          FROM Pages p
                          WHERE p.Is_Deleted = 0 AND p.Name LIKE @Name;";
            var parameter = new DynamicParameters();
            parameter.Add("Name", $"%{name}%");
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<PageQuery>(query, parameter);
                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<PageQueryDetail> GetByID(Guid id)
        {
            var query = @"SELECT p.Id, p.Name, p.Avatar, p.Wallpaper, p.Created_At,
                          up.User_Id,  up.Role, up.Is_Follow, ud.FirstName, ud.LastName, ud.Avatar as User_Avatar, up.Joined_At
                          FROM Pages p
                          LEFT JOIN User_Page up ON p.Id = up.Page_Id
                          LEFT JOIN User_Details ud ON ud.User_Id = up.User_Id
                          WHERE p.Is_Deleted = 0 AND up.Is_Deleted = 0 AND p.Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            try
            {
                var pageDict = new Dictionary<Guid, PageQueryDetail>();
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PageQueryDetail, UserPageQuery, PageQueryDetail>(
                        query,
                        map: (page_detail, user) =>
                        {
                            if (!pageDict.TryGetValue(page_detail.Id, out var pageEntry))
                            {
                                pageEntry = page_detail;
                                pageDict.Add(page_detail.Id, pageEntry);
                            }

                            if (user != null && !pageEntry.Users.Any((item) => item.User_Id == user.User_Id))
                            {
                                pageEntry.Users.Add(user);
                            }
                            return pageEntry;
                        },
                        parameter,
                        splitOn: "User_Id"
                        );
                }
                return pageDict.Values.ToList()[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("{id}/followers")]
        public async Task<IEnumerable<UserPageQuery>> GetUserFollowPage(Guid id)
        {
            var query = @"SELECT up.User_Id,  up.Role, up.Is_Follow, ud.FirstName, ud.LastName, ud.Avatar as User_Avatar, up.Joined_At
                          FROM User_Page up
                          INNER JOIN User_Details ud ON ud.User_Id = up.User_Id
                          WHERE up.Page_Id = @Id AND up.Is_Deleted = 0 AND up.Role LIKE 'Follower';";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<UserPageQuery>(query, parameter);
                return result;
            }
        }

        [HttpGet("{id}/admins")]
        public async Task<IEnumerable<UserPageQuery>> GetUserManagePage(Guid id)
        {
            var query = @"SELECT up.User_Id, up.Role, up.Is_Follow, ud.FirstName, ud.LastName, ud.Avatar as User_Avatar, up.Joined_At
                          FROM User_Page up
                          INNER JOIN User_Details ud ON ud.User_Id = up.User_Id
                          WHERE up.Page_Id = @Id AND up.Is_Deleted = 0 AND up.Role NOT LIKE 'Follower';";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<UserPageQuery>(query, parameter);
                return result;
            }
        }

        [HttpGet("{id}/posts")]
        public async Task<IEnumerable<PostQuery>> GetPostsInPage(Guid id)
        {
            var query = @"
            SELECT 
                p.Id,
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

            FROM 
                Posts p
            INNER JOIN 
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
            WHERE 
                p.Is_Deleted = 0 AND p.In_Group = 0 AND p.Destination_Id = @Id;";

            try
            {
                var postDict = new Dictionary<Guid, PostQuery>();
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostQuery, AttachmentQuery, PostSurveyQuery, PostSurveyItemQuery, PostVoteQuery, PostCommentQuery, PostReactQuery, PostQuery>(
                    query,
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
                    splitOn: "Attachment_Id, Survey_Id, SurveyItem_Id, Vote_UserId, Comment_Id, React_Type");
                    return postDict.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePageCommand command)
        {
            var query = @"INSERT INTO Pages (Id, Created_At, Updated_At, Is_Deleted, Name, Avatar, Wallpaper)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Name, @Avatar, @Wallpaper);";
            var queryOwner = @"INSERT INTO User_Page (User_Id, Page_Id, Role ,Is_Follow, Joined_At, Updated_At, Is_Deleted)
                          VALUES
                          (@User_Id, @Page_Id, 'Owner', 1 ,GETDATE(), GETDATE(), 0);";
            var parameters = new DynamicParameters();
            parameters.Add("Name", command.Name);
            parameters.Add("Avatar", command.Avatar);
            parameters.Add("Wallpaper", command.Wallpaper);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = await connection.QuerySingleAsync<Guid>(query, parameters, transaction);
                        parameters = new DynamicParameters();
                        parameters.Add("User_Id", command.User_Id);
                        parameters.Add("Page_Id", result);
                        await connection.ExecuteAsync(queryOwner, parameters, transaction);
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

        [HttpPost("{id}/user")]
        public async Task<IActionResult> AddPageUser(Guid id, ModifyPageUserCommand command)
        {
            //check if exist
            var existQuery = @"SELECT COUNT(1) FROM User_Page
                               WHERE User_Id = @User_Id AND Page_Id = @Page_Id;";
            var query = @"INSERT INTO User_Page (User_Id, Page_Id, Role, Is_Follow, Joined_At, Updated_At, Is_Deleted)
                          VALUES
                          (@User_Id, @Page_Id, @Role, 1, GETDATE(), GETDATE(), 0)";
            var parameters = new DynamicParameters();
            parameters.Add("Role", command.Role);
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Page_Id", command.Page_Id);
            using (var connection = _context.CreateConnection())
            {
                bool existed = await connection.ExecuteScalarAsync<bool>(existQuery, parameters);
                if (existed)
                {
                    return BadRequest("Existed Connection between User and Page");
                }
                else
                {
                    await connection.ExecuteAsync(query, parameters);
                    return Ok();

                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, CreatePageCommand command)
        {
            var query = @"UPDATE Pages
                          SET Name = @Name, Avatar = @Avatar, Wallpaper = @Wallpaper 
                          WHERE Page_Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Name", command.Name);
            parameters.Add("Avatar", command.Avatar);
            parameters.Add("Wallpaper", command.Wallpaper);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpPut("{id}/user")]
        public async Task<IActionResult> ModifyPageUser(Guid id, ModifyPageUserCommand command)
        {
            var query = @"UPDATE User_Page
                          SET Role = @Role
                          WHERE Page_Id = @Id AND User_Id = @User_Id";
            var parameters = new DynamicParameters();
            parameters.Add("Role", command.Role);
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Page_Id", command.Page_Id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePage(Guid id)
        {
            var query = "UPDATE Pages SET Is_Deleted = 1, Updated_At = GETDATE() WHERE Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }

        [HttpDelete("{id}/users")]
        public async Task<IActionResult> RemoveUserFromPage(Guid id, ModifyFollowPageCommand command)
        {
            var query = "UPDATE User_Page SET Is_Deleted = 1, Updated_At = GETDATE() WHERE User_Id = @User_Id AND Page_Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }



    }
}

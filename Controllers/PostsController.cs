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
                    var result = await connection.QueryAsync<PostBasicQuery, string, PostCommentQuery, PostReactQuery, PostBasicQuery>(
                    query,
                    map: (post, mediaUrl, comment, react) =>
                    {
                        if (!postDict.TryGetValue(post.PostId, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.PostId, postEntry);
                        }

                        if (!string.IsNullOrEmpty(mediaUrl) && !postEntry.MediaUrls.Any((item) => item == mediaUrl))
                        {
                            postEntry.MediaUrls.Add(mediaUrl);
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

                    splitOn: "MediaUrl,CommentId, ReactId");
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
        public async Task<PostBasicQuery> Get(Guid id)
        {
            var query = @"
            SELECT 
                p.Id AS PostId,
                p.Content AS PostContent,
                p.PostType,
                u.Id AS UserId,
                u.UserName,
                u.Email,
                u.AvatarUrl
            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.OwnerId = u.Id
            WHERE 
                p.IsDeleted = 0 AND p.Id=@Id;"

            + @"SELECT URL 
                FROM MediaItems 
                WHERE PostId = @Id;"

            + @"SELECT 
                c.Id AS CommentId,
                c.Content AS CommentContent,
                c.MediaURL AS CommentMediaUrl,
                c.CreatedAt AS CommentCreatedAt,
                c.UserId AS CommentUserId,
                uc.UserName AS CommentUserName,
                uc.AvatarUrl AS CommentUserAvatarUrl
                FROM Comments c
                JOIN
                Users uc ON c.UserId = uc.Id
                WHERE 
                c.IsDeleted = 0 AND c.PostId = @Id;"

             + @"SELECT 
                 r.Id AS ReactId,
                 r.ReactType,
                 ur.Id AS ReactUserId,
                 ur.UserName AS ReactUserName,
                 ur.AvatarUrl AS ReactUserAvatar
                 FROM Reacts r
                 JOIN Users ur ON r.UserId = ur.Id
                 WHERE 
                 r.IsDeleted = 0 AND r.PostId = @Id";

            using (var connection = _context.CreateConnection())
            {
                using (var multi = await connection.QueryMultipleAsync(query, new { id }))
                {
                    var post = await multi.ReadSingleOrDefaultAsync<PostBasicQuery>();
                    if (post != null)
                    {
                        foreach (string mediaUrl in (await multi.ReadAsync<string>()).ToList())
                        {
                            if (!string.IsNullOrEmpty(mediaUrl) && !post.MediaUrls.Any((item) => item == mediaUrl))
                            {
                                post.MediaUrls.Add(mediaUrl);
                            }
                        }
                        foreach (PostCommentQuery comment in (await multi.ReadAsync<PostCommentQuery>()).ToList())
                        {

                            if (comment != null && !post.Comments.Any((item) => item.CommentId == comment.CommentId))
                            {
                                post.Comments.Add(comment);
                            }
                        }
                        foreach (PostReactQuery react in (await multi.ReadAsync<PostReactQuery>()).ToList())
                        {

                            if (react != null && !post.Reacts.Any((item) => item.ReactId == react.ReactId))
                            {
                                post.Reacts.Add(react);
                            }
                        }

                    }
                    return post;
                }
            }
        }

        // POST api/<PostController>
        [HttpPost]
        public async Task<IActionResult> Post(CreatePostRequest request)
        {
            var userId = Guid.Parse(request.UserId);
            var newPost = request.NewPost;
            var query = @"INSERT INTO Posts (Id, CreatedAt, UpdatedAt, IsDeleted, DestinationId, IsNotification, PostType, PostDestination, Content, OwnerId)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, 'dest1', @IsNotification, @PostType, 'All', @Content, @UserId)";
            var parameters = new DynamicParameters();
            parameters.Add("IsNotification", newPost.IsNotification, DbType.Boolean);
            parameters.Add("PostType", newPost.PostType, DbType.String);
            parameters.Add("PostDestination", newPost.PostDestination, DbType.String);
            parameters.Add("Content", newPost.Content, DbType.String);
            parameters.Add("UserId", userId, DbType.Guid);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
            return Ok();
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
            var query = "DELETE FROM Posts WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }
    }
}

//var query = @"
//SELECT 
//    p.Id AS PostId,
//    p.Content AS PostContent,
//    p.PostType,
//    u.Id AS UserId,
//    u.UserName,
//    u.Email,
//    u.AvatarUrl,
//    m.URL AS MediaUrl
//FROM 
//    Posts p
//INNER JOIN 
//    Users u ON p.OwnerId = u.Id
//LEFT JOIN
//    MediaItems m ON p.Id = m.PostId
//WHERE 
//    p.IsDeleted = 0 AND p.Id=@Id;";

//try
//{
//    var postDict = new Dictionary<Guid, PostBasicQuery>();

//    using (var connection = _context.CreateConnection())
//    {
//        var result = await connection.QueryAsync<PostBasicQuery, string, PostBasicQuery>(
//        query,
//        (post, mediaUrl) =>
//        {
//            if (!postDict.TryGetValue(post.PostId, out var postEntry))
//            {
//                postEntry = post;
//                postDict.Add(post.PostId, postEntry);
//            }

//            // Add the media URL to the post's list of media
//            if (!string.IsNullOrEmpty(mediaUrl))
//            {
//                postEntry.MediaUrls.Add(mediaUrl);
//            }
//            return postEntry;
//        },
//        new { Id = id },
//        splitOn: "MediaUrl");

//        // Return the result as a list of posts with media URLs
//        return postDict.Values.FirstOrDefault();
//    } // Execute query and map result to DTOs
//}
//catch
//{
//    throw;
//}

using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;
using EnVietSocialNetWorkAPI.Entities.Queries;
using EnVietSocialNetWorkAPI.Old;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnVietSocialNetWorkAPI.Controllers
{
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
        public async Task<IEnumerable<PostBasicQuery>> Get()
        {
            string query = @"
            SELECT 
                p.Id AS PostId,
                p.Content AS PostContent,
                p.PostType,
                u.Id AS UserId,
                u.UserName,
                u.Email,
                u.AvatarUrl,
                m.URL AS MediaUrl
            FROM 
                Posts p
            INNER JOIN 
                Users u ON p.OwnerId = u.Id
            LEFT JOIN
                MediaItems m ON p.Id = m.PostId
            WHERE 
                p.IsDeleted = 0;";

            try
            {
                var postDict = new Dictionary<Guid, PostBasicQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<PostBasicQuery, string, PostBasicQuery>(
                    query,
                    (post, mediaUrl) =>
                    {
                        // Ensure we have a dictionary entry for the post
                        if (!postDict.TryGetValue(post.PostId, out var postEntry))
                        {
                            postEntry = post;
                            postDict.Add(post.PostId, postEntry);
                        }

                        // Add the media URL to the post's list of media
                        if (!string.IsNullOrEmpty(mediaUrl))
                        {
                            postEntry.MediaUrls.Add(mediaUrl);
                        }

                        return postEntry;
                    },
                    splitOn: "MediaUrl");

                    // Return the list of all posts with media
                    return postDict.Values.ToList();
                }
                // Execute query and map result to DTOs

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
                WHERE PostId = @Id";

            using (var connection = _context.CreateConnection())
            using (var multi = await connection.QueryMultipleAsync(query, new { id }))
            {
                var post = await multi.ReadSingleOrDefaultAsync<PostBasicQuery>();
                if (post != null)
                    post.MediaUrls = (await multi.ReadAsync<string>()).ToList();
                return post;
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

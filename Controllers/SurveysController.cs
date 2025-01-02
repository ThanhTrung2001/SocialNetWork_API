using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SurveysController : ControllerBase
    {
        private readonly DapperContext _context;
        public SurveysController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Survey>> Get()
        {
            var query = "SELECT * FROM Surveys WHERE IsDeleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Survey>(query);
                return result;
            }
        }

        [HttpGet("post/{postId}")]
        public async Task<Survey> GetByPostId(Guid postId)
        {
            var query = "SELECT * FROM Surveys WHERE IsDeleted = 0 AND p.PostId = @PostId;";
            var parameters = new DynamicParameters();
            parameters.Add("PostId", postId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<Survey>(query, parameters);
                return result;
            }
        }

        [HttpPost("post/{postId}")]
        public async Task<IActionResult> CreateByPostId(Guid postId, NewSurvey survey)
        {
            var query = @"INSERT INTO Surveys (ID, CreatedAt, UpdatedAt, IsDeleted, ExpiredIn, PostId, Question)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @ExpiredIn, @PostId, @Question)";
            var parameters = new DynamicParameters();

            parameters.Add("ExpiredIn", survey.ExpiredIn, DbType.DateTime);
            parameters.Add("PostId", postId, DbType.Guid);
            parameters.Add("Question", survey.Question, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSurvey(Guid id, NewSurvey survey)
        {
            var query = @"UPDATE Surveys SET ExpiredIn = @ExpiredIn, Question = @Question WHERE Id = @Id;";
            var parameters = new DynamicParameters();

            parameters.Add("ExpiredIn", survey.ExpiredIn, DbType.DateTime);
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("Question", survey.Question, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update Surveys SET isDeleted = 1 WHERE Id = @Id";
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

using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Models.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyVotesController : ControllerBase
    {
        private readonly DapperContext _context;
        public SurveyVotesController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("option/{surveyItemId}")]
        public async Task<IEnumerable<SurveyVoteQuery>> GetByOptionId(Guid surveyItemId)
        {
            var query = "SELECT * FROM UserVote WHERE SurveyItemId = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", surveyItemId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<SurveyVoteQuery>(query, parameters);
                return result;
            }
        }

        [HttpPost("option/{surveyItemId}")]
        public async Task<IActionResult> CreateByOptionId(Guid surveyItemId, CreateSurveyVoteCommand vote)
        {
            var query = @"INSERT INTO UserVote (SurveyItemId, UserId)
                          VALUES
                          (@SurveyItemId, @UserId)";
            var parameters = new DynamicParameters();
            parameters.Add("SurveyItemId", surveyItemId, DbType.Guid);
            parameters.Add("UserId", vote.UserId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok(result);
            }
        }

        [HttpDelete("option/{surveyItemId}")]
        public async Task<IActionResult> Delete(Guid surveyItemId, CreateSurveyVoteCommand command)
        {
            var query = "DELETE FROM userVote WHERE UserId = @UserId AND SurveyItemId = @SurveyItemId";
            var parameters = new DynamicParameters();
            parameters.Add("UserId", command.UserId, DbType.Int32);
            parameters.Add("SurveyItemId", surveyItemId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

    }
}

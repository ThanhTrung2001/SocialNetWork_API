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
            var query = "SELECT * FROM SurveyVotes WHERE OptionId = @Id";
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
            var query = @"INSERT INTO SurveyVotes (OptionId, UserId, VotedAt)
                          VALUES
                          (@OptionId, @UserId, GETDATE())";
            var parameters = new DynamicParameters();
            parameters.Add("OptionId", surveyItemId, DbType.Guid);
            parameters.Add("UserId", vote.UserId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok(result);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var query = "DELETE FROM SurveyVotes WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

    }
}

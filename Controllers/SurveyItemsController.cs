using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyItemsController : ControllerBase
    {
        private readonly DapperContext _context;

        public SurveyItemsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("survey/{surveyId}")]
        public async Task<IEnumerable<SurveyItem>> GetBySurveyId(Guid surveyId)
        {
            var query = "SELECT * FROM SurveyItems WHERE IsDeleted = 0 AND SurveyId = @SurveyId";
            var parameters = new DynamicParameters();
            parameters.Add("SurveyId", surveyId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<SurveyItem>(query, parameters);
                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<SurveyItem> GetById(Guid id)
        {
            var query = "SELECT * FROM SurveyItems WHERE IsDelted = 0 AND Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<SurveyItem>(query, parameters);
                return result;
            }
        }

        [HttpPost("survey/{surveyId}")]
        public async Task<IActionResult> CreateBySurveyId(Guid surveyId, string content)
        {
            var query = @"INSERT INTO SurveyItems (ID, CreatedAt, UpdatedAt, IsDeleted, Content, SurveyId, Votes)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Content, @SurveyId, 0)";
            var parameters = new DynamicParameters();
            parameters.Add("SurveyId", surveyId, DbType.Guid);
            parameters.Add("Content", content, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSurveyItem(Guid id, string content)
        {
            var query = "UPDATE SurveyItems SET Content = @Content WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("Content", content, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update SurveyItems SET isDeleted = 1 WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
                return Ok();
            }
        }
    }
}

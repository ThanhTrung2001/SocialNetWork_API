using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Models.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyItemQuerysController : ControllerBase
    {
        private readonly DapperContext _context;

        public SurveyItemQuerysController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("survey/{surveyId}")]
        public async Task<IEnumerable<SurveyItemQuery>> GetBySurveyId(Guid surveyId)
        {
            var query = "SELECT * FROM SurveyItems WHERE IsDeleted = 0 AND SurveyId = @SurveyId";
            var parameters = new DynamicParameters();
            parameters.Add("SurveyId", surveyId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<SurveyItemQuery>(query, parameters);
                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<SurveyItemQuery> GetById(Guid id)
        {
            var query = "SELECT * FROM SurveyItems WHERE IsDelted = 0 AND Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<SurveyItemQuery>(query, parameters);
                return result;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSurveyItemQuery(Guid id, string content)
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
            var query = "Update SurveyItemQuerys SET isDeleted = 1, UpdatedAt = GETDATE() WHERE Id = @Id";
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

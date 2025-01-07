using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Model.Commands;
using EnVietSocialNetWorkAPI.Model.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SurveysController : ControllerBase
    {
        private readonly DapperContext _context;
        public SurveysController(DapperContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<IEnumerable<SurveyQuery>> Get()
        //{
        //    var query = "SELECT * FROM Surveys WHERE IsDeleted = 0;";
        //    using (var connection = _context.CreateConnection())
        //    {
        //        var result = await connection.QueryAsync<SurveyQuery>(query);
        //        return result;
        //    }
        //}

        [HttpGet("post/{postId}")]
        public async Task<SurveyQuery> GetByPostId(Guid postId)
        {
            var query = "SELECT ID, CreatedAt, ExpiredAt, Total, SurveyTypeId, PostId, Question FROM Surveys WHERE IsDeleted = 0 AND p.PostId = @PostId;";
            var parameters = new DynamicParameters();
            parameters.Add("PostId", postId, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<SurveyQuery>(query, parameters);
                return result;
            }
        }

        [HttpPost("post/{postId}")]
        public async Task<IActionResult> CreateByPostId(Guid postId, CreateSurveyCommand survey)
        {
            var query = @"INSERT INTO Surveys (ID, CreatedAt, UpdatedAt, IsDeleted, ExpiredAt, Total, SurveyTypeId, PostId, Question)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @ExpiredAt, 0, @SurveyTypeId , @PostId, @Question)";
            var querySurveyItem = @"INSERT INTO SurveyItems (ID, CreatedAt, UpdatedAt, IsDeleted, OptionName, SurveyId, Total)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Content, @SurveyId, 0)";

            var parameters = new DynamicParameters();

            parameters.Add("ExpiredAt", survey.ExpiredAt, DbType.DateTime);
            parameters.Add("PostId", postId, DbType.Guid);
            parameters.Add("Question", survey.Question, DbType.String);
            parameters.Add("SurveyTypeId", survey.SurveyTypeId);

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = await connection.QuerySingleAsync(query, parameters, transaction);
                        foreach (var item in survey.SurveyItems)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("SurveyId", result);
                            parameters.Add("OptionName", item.OptionName);
                            await connection.ExecuteAsync(querySurveyItem, parameters, transaction);
                            transaction.Commit();
                        }
                        return Ok();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return BadRequest(e.Message);
                    }

                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSurvey(Guid id, CreateSurveyCommand survey)
        {
            var query = @"UPDATE Surveys SET ExpiredAt = @ExpiredAt, Question = @Question WHERE Id = @Id;";
            var parameters = new DynamicParameters();

            parameters.Add("ExpiredAt", survey.ExpiredAt, DbType.DateTime);
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

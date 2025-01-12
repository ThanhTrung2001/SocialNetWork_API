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
    public class SurveyVotesController : ControllerBase
    {
        private readonly DapperContext _context;
        public SurveyVotesController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("option/{surveyItem_Id}")]
        public async Task<IActionResult> GetByOptionId(Guid surveyItem_Id)
        {
            var query = "SELECT * FROM User_SurveyItem_Vote WHERE SurveyItem_Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", surveyItem_Id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<SurveyVoteQuery>(query, parameters);
                    return Ok(ResponseModel<IEnumerable<SurveyVoteQuery>>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<SurveyVoteQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateByOptionId(CreateSurveyVoteCommand vote)
        {
            var query = @"INSERT INTO User_SurveyItem_Vote (SurveyItem_Id, User_Id)
                          VALUES
                          (@SurveyItem_Id, @User_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("SurveyItem_Id", vote.SurveyItem_Id, DbType.Guid);
            parameters.Add("User_Id", vote.User_Id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameters);
                    return Ok(ResponseModel<string>.Success("Success."));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<Guid>.Failure(ex.Message));
                }
            }
        }

        [HttpDelete("option/{surveyItem_Id}")]
        public async Task<IActionResult> Delete(CreateSurveyVoteCommand command)
        {
            var query = "DELETE FROM User_SurveyItem_Vote WHERE User_Id = @User_Id AND SurveyItem_Id = @SurveyItem_Id";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("SurveyItem_Id", command.SurveyItem_Id);
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

    }
}

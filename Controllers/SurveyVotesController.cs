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
            var query = @"SELECT 
                          usv.User_Id AS Vote_UserId,
                          ud.FirstName AS Vote_FirstName,
                          ud.LastName AS Vote_LastName,
                          ud.Avatar AS Vote_Avatar
                          FROM User_SurveyItem_Vote usv
                          INNER JOIN User_Details ud ON usv.User_Id = ud.User_Id
                          WHERE SurveyItem_Id = @Id";
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
            var surveyItemQuery = @"UPDATE Survey_Items 
                                SET Total_Vote = Total_Vote + 1 
                                WHERE Id = @SurveyItem_Id";
            var surveyQuery = @"UPDATE Surveys 
                                SET Total_Vote = Total_Vote + 1 
                                WHERE Id = (SELECT Survey_Id from Survey_Items WHERE Id = @SurveyItem_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("SurveyItem_Id", vote.SurveyItem_Id, DbType.Guid);
            parameters.Add("User_Id", vote.User_Id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameters, transaction);
                        await connection.ExecuteAsync(surveyItemQuery, parameters, transaction);
                        await connection.ExecuteAsync(surveyQuery, parameters, transaction);
                        transaction.Commit();
                        return Ok(ResponseModel<string>.Success("Success."));

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<Guid>.Failure(ex.Message));
                    }
                }
            }
        }

        [HttpDelete("option/{surveyItem_Id}")]
        public async Task<IActionResult> Delete(CreateSurveyVoteCommand command)
        {
            var query = "DELETE FROM User_SurveyItem_Vote WHERE User_Id = @User_Id AND SurveyItem_Id = @SurveyItem_Id";
            var surveyItemQuery = @"UPDATE Survey_items 
                                SET Total_Vote = Total_Vote - 1 
                                WHERE Id = @SurveyItem_Id";
            var surveyQuery = @"UPDATE Surveys 
                                SET Total_Vote = Total_Vote -1
                                WHERE Id = (SELECT Survey_Id from Survey_Items WHERE Id = @SurveyItem_Id)";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("SurveyItem_Id", command.SurveyItem_Id);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameters, transaction);
                        await connection.ExecuteAsync(surveyItemQuery, parameters, transaction);
                        await connection.ExecuteAsync(surveyQuery, parameters, transaction);
                        transaction.Commit();
                        return Ok(ResponseModel<string>.Success("Success."));

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<Guid>.Failure(ex.Message));
                    }
                }
            }
        }

    }
}

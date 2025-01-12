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
    public class SurveysController : ControllerBase
    {
        private readonly DapperContext _context;
        public SurveysController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet("survey/{survey_Id}")]
        public async Task<IActionResult> GetBysurvey_Id(Guid survey_Id)
        {
            var query = @"SELECT
                            s.ID, s.Created_At, s.Expired_At, s.Total_Vote, s.Survey_Type, s.survey_Id, s.Question,
                            
                            si.Id AS SurveyItem_Id,
                            si.Option_Name AS SurveyItem_Name,
                            si.Total_Vote AS Item_Total,

                            uv.User_Id AS Vote_UserId,
                            udv.FirstName AS Vote_FirstName,
                            udv.LastName AS Vote_LastName,
                            udv.Avatar AS Vote_Avatar,

                          FROM Surveys s
                          INNER JOIN Survey_Items si ON s.Id = si.Survey_Id
                          LEFT JOIN User_SurveyItem_vote uv ON uv.SurveyItem_Id = si.Id
                          LEFT JOIN User_Details udv ON udv.User_Id = uv.User_Id
                          WHERE Is_Deleted = 0 AND p.survey_Id = @survey_Id;";

            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("Id", survey_Id);
                var surveyDict = new Dictionary<Guid, SurveyQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<SurveyQuery, SurveyItemQuery, SurveyVoteQuery, SurveyQuery>(
                    query,
                    map: (survey, surveyItem, vote) =>
                    {
                        if (!surveyDict.TryGetValue(survey.Id, out var surveyEntry))
                        {
                            surveyEntry = survey;
                            surveyDict.Add(survey.Id, surveyEntry);
                        }

                        if (surveyItem != null && !surveyEntry.SurveyItems.Any((item) => item.SurveyItem_Id == surveyItem.SurveyItem_Id))
                        {
                            surveyEntry.SurveyItems.Add(surveyItem);
                            var result = surveyEntry.SurveyItems.FirstOrDefault((x) => x.SurveyItem_Id == surveyItem.SurveyItem_Id);
                            if (vote != null && !result.Votes.Any((item) => item.Vote_UserId == vote.Vote_UserId))
                            {
                                result.Votes.Add(vote);
                            }
                        }
                        return surveyEntry;
                    },

                    parameter,
                    splitOn: "SurveyItem_Id, Vote_UserId");
                    return Ok(ResponseModel<IEnumerable<SurveyQuery>>.Success(surveyDict.Values.ToList()));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<SurveyQuery>>.Failure(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBySurveyId(CreateSurveyCommand survey)
        {
            var query = @"INSERT INTO Surveys (ID, Created_At, Updated_At, Is_Deleted, Expired_At, Total_Vote, Survey_Type, Question)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Expired_At, 0, @Survey_Type ,@Question)";
            var querySurveyItem = @"INSERT INTO SurveyItems (ID, Created_At, Updated_At, Is_Deleted, Option_Name, Total_Vote)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Content, 0)";

            var parameters = new DynamicParameters();

            parameters.Add("Expired_At", survey.Expired_At, DbType.DateTime);
            parameters.Add("Question", survey.Question, DbType.String);
            parameters.Add("Survey_Type", survey.Survey_Type);

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
                            parameters.Add("Survey_Id", result);
                            parameters.Add("Option_Name", item.Option_Name);
                            await connection.ExecuteAsync(querySurveyItem, parameters, transaction);
                            transaction.Commit();
                        }
                        return Ok(ResponseModel<Guid>.Success(result));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(ResponseModel<Guid>.Failure(ex.Message));
                    }

                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSurvey(Guid id, CreateSurveyCommand survey)
        {
            var query = @"UPDATE Surveys SET Expired_At = @Expired_At, Question = @Question WHERE Id = @Id;";
            var parameters = new DynamicParameters();

            parameters.Add("Expired_At", survey.Expired_At, DbType.DateTime);
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("Question", survey.Question, DbType.String);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update Surveys SET Is_Deleted = 1 WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
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

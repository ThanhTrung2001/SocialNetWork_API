using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;
using System.Data;

namespace EV.DataAccess.Repositories
{
    public class SurveyRepository
    {
        private readonly DatabaseContext _context;

        public SurveyRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<SurveyQuery>>> GetByPostId(Guid id)
        {
            var query = @"SELECT
                            s.ID, s.Created_At, s.Expired_At, s.Total_Vote, s.Survey_Type, s.Question,
                            
                            si.Id AS SurveyItem_Id,
                            si.Option_Name,
                            si.Total_Vote,

                            uv.User_Id AS Vote_UserId,
                            udv.FirstName AS Vote_FirstName,
                            udv.LastName AS Vote_LastName,
                            udv.Avatar AS Vote_Avatar

                          FROM Surveys s
                          INNER JOIN Survey_Items si ON s.Id = si.Survey_Id
                          LEFT JOIN User_SurveyItem_Vote uv ON uv.SurveyItem_Id = si.Id
                          LEFT JOIN User_Details udv ON udv.User_Id = uv.User_Id
                          WHERE s.Is_Deleted = 0 AND s.Post_Id = @Id;";

            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
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

                        if (surveyItem != null)
                        {
                            var existingItem = surveyEntry.SurveyItems
                                .FirstOrDefault(item => item.SurveyItem_Id == surveyItem.SurveyItem_Id);

                            if (existingItem == null)
                            {
                                existingItem = surveyItem;
                                surveyEntry.SurveyItems.Add(existingItem);
                            }

                            if (vote != null && !existingItem.Votes.Any(v => v.Vote_UserId == vote.Vote_UserId))
                            {
                                existingItem.Votes.Add(vote);
                            }
                        }
                        return surveyEntry;
                    },

                    parameter,
                    splitOn: "SurveyItem_Id, Vote_UserId");
                    return ResponseModel<IEnumerable<SurveyQuery>>.Success(surveyDict.Values.ToList());
                }
            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<SurveyQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<IEnumerable<SurveyQuery>>> GetById(Guid id)
        {
            var query = @"SELECT
                            s.ID, s.Created_At, s.Expired_At, s.Total_Vote, s.Survey_Type, s.Question,
                            
                            si.Id AS SurveyItem_Id,
                            si.Option_Name,
                            si.Total_Vote,

                            uv.User_Id AS Vote_UserId,
                            udv.FirstName AS Vote_FirstName,
                            udv.LastName AS Vote_LastName,
                            udv.Avatar AS Vote_Avatar

                          FROM Surveys s
                          INNER JOIN Survey_Items si ON s.Id = si.Survey_Id
                          LEFT JOIN User_SurveyItem_Vote uv ON uv.SurveyItem_Id = si.Id
                          LEFT JOIN User_Details udv ON udv.User_Id = uv.User_Id
                          WHERE s.Is_Deleted = 0 AND s.Id = @Id;";

            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);
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

                        if (surveyItem != null)
                        {
                            var existingItem = surveyEntry.SurveyItems
                                .FirstOrDefault(item => item.SurveyItem_Id == surveyItem.SurveyItem_Id);

                            if (existingItem == null)
                            {
                                existingItem = surveyItem;
                                surveyEntry.SurveyItems.Add(existingItem);
                            }

                            if (vote != null && !existingItem.Votes.Any(v => v.Vote_UserId == vote.Vote_UserId))
                            {
                                existingItem.Votes.Add(vote);
                            }
                        }
                        return surveyEntry;
                    },

                    parameter,
                    splitOn: "SurveyItem_Id, Vote_UserId");
                    return ResponseModel<IEnumerable<SurveyQuery>>.Success(surveyDict.Values.ToList());
                }
            }
            catch (Exception ex)
            {
                return ResponseModel<IEnumerable<SurveyQuery>>.Failure(ex.Message)!;
            }
        }

        public async Task<ResponseModel<Guid>> Create(CreateSurveyCommand command)
        {
            var query = @"INSERT INTO Surveys (ID, Created_At, Updated_At, Is_Deleted, Expired_At, Total_Vote, Post_Id, Survey_Type, Question)
                          OUTPUT Inserted.Id
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Expired_At, 0, @Post_Id ,@Survey_Type ,@Question)";
            var querySurveyItem = @"INSERT INTO Survey_Items (ID, Created_At, Updated_At, Is_Deleted, Option_Name, Total_Vote, Survey_Id)
                                  VALUES
                                  (NEWID(), GETDATE(), GETDATE(), 0, @Option_Name, 0)";

            var parameters = new DynamicParameters();
            parameters.Add("Post_Id", command.Post_Id);
            parameters.Add("Expired_At", command.Expired_At, DbType.DateTime);
            parameters.Add("Question", command.Question, DbType.String);
            parameters.Add("Survey_Type", command.Survey_Type);

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var result = await connection.QuerySingleAsync(query, parameters, transaction);
                        foreach (var item in command.SurveyItems)
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("Survey_Id", result);
                            parameters.Add("Option_Name", item.Option_Name);
                            var rowEffect = await connection.ExecuteAsync(querySurveyItem, parameters, transaction);
                            transaction.Commit();
                        }
                        return ResponseModel<Guid>.Success(result);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ResponseModel<Guid>.Failure(ex.Message)!;
                    }

                }
            }
        }

        public async Task<ResponseModel<string>> Edit(Guid id, EditSurveyCommand command)
        {
            var query = @"UPDATE Surveys SET Expired_At = @Expired_At, Question = @Question, Updated_At = GETDATE() WHERE Id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("Expired_At", command.Expired_At, DbType.DateTime);
            parameters.Add("Question", command.Question, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;

                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Delete(Guid id)
        {
            var query = "Update Surveys SET Is_Deleted = 1 WHERE Id = @Id";
            var querySurveyItem = "Update Survey_Items SET Updated_At = GETDATE(), Is_Deleted = 1 WHERE Survey_Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    var rowSurveyItemEffect = await connection.ExecuteAsync(querySurveyItem, parameters);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }
    }
}

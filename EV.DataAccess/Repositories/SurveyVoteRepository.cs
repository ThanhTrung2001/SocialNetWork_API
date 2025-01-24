using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;
using System.Data;

namespace EV.DataAccess.Repositories
{
    public class SurveyVoteRepository
    {
        private readonly DatabaseContext _context;

        public SurveyVoteRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<SurveyVoteQuery>>> GetBySurveyItemID(Guid id)
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
            parameters.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<SurveyVoteQuery>(query, parameters);
                    return ResponseModel<IEnumerable<SurveyVoteQuery>>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<SurveyVoteQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Create(CreateSurveyVoteCommand command)
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
            parameters.Add("SurveyItem_Id", command.SurveyItem_Id, DbType.Guid);
            parameters.Add("User_Id", command.User_Id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var rowEffect = await connection.ExecuteAsync(query, parameters, transaction);
                        var rowSurveyItemEffect = await connection.ExecuteAsync(surveyItemQuery, parameters, transaction);
                        var rowSurveyEffect = await connection.ExecuteAsync(surveyQuery, parameters, transaction);
                        transaction.Commit();
                        return (rowEffect > 0 && rowSurveyItemEffect > 0 && rowSurveyEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ResponseModel<string>.Failure(ex.Message)!;
                    }
                }
            }
        }

        public async Task<ResponseModel<string>> Delete(CreateSurveyVoteCommand command)
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
                        var rowEffect = await connection.ExecuteAsync(query, parameters, transaction);
                        var rowSurveyItemEffect = await connection.ExecuteAsync(surveyItemQuery, parameters, transaction);
                        var rowSurveyEffect = await connection.ExecuteAsync(surveyQuery, parameters, transaction);
                        transaction.Commit();
                        return (rowEffect > 0 && rowSurveyItemEffect > 0 && rowSurveyEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ResponseModel<string>.Failure(ex.Message)!;
                    }
                }
            }
        }
    }


}

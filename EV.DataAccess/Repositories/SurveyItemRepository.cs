using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;

namespace EV.DataAccess.Repositories
{
    public class SurveyItemRepository
    {
        private readonly DatabaseContext _context;

        public SurveyItemRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<ResponseModel<string>> Create(Guid id, EditSurveyItemCommand command)
        {
            var query = @"INSERT INTO Survey_Items (ID, Created_At, Updated_At, Is_Deleted, Option_Name, Total_Vote, Survey_Id)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @Option_Name, 0, @Id)";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Option_Name", command.Option_Name);
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

        public async Task<ResponseModel<string>> Edit(Guid id, EditSurveyItemCommand command)
        {
            var query = @"UPDATE Survey_Items SET Option_Name = @Option_Name, Updated_At = GETDATE() WHERE Id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Option_Name", command.Option_Name);
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
            var query = @"Update Surveys
                          SET Total_Vote = Total_Vote - (SELECT Total_Vote FROM Survey_Items WHERE Id = @Id ) 
                          WHERE Id = (SELECT Survey_Id FROM Survey_Items WHERE Id = @Id );";
            var querySurveyItem = "Update Survey_Items SET Updated_At = GETDATE(), Is_Deleted = 1 WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var rowEffect = await connection.ExecuteAsync(query, parameters, transaction);
                        var rowSurveyEffect = await connection.ExecuteAsync(querySurveyItem, parameters, transaction);
                        transaction.Commit();
                        return (rowEffect > 0 && rowSurveyEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
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

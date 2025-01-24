using Dapper;
using Ev.Model.Handlers.Queries;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;

namespace EV.DataAccess.Repositories
{
    public class TagRepository
    {
        private readonly DatabaseContext _context;

        public TagRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<TagQuery>>> GetAll()
        {
            var query = "SElECT Id, Tag_Name FROM Tags";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<TagQuery>(query);
                    return ResponseModel<IEnumerable<TagQuery>>.Success(result);
                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<TagQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Create(CreateTagCommand command)
        {
            var query = @"INSERT INTO Tags (Tag_Name)
                          VALUES
                          (@Tag_Name);";
            var parameter = new DynamicParameters();
            parameter.Add("Tag_Name", command.Tag_Name);
            using (var connection = _context.CreateConnection())
            {

                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameter);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Create Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;

                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Edit(int id, CreateTagCommand command)
        {
            var query = "UPDATE Tags SET Tag_Name = @Tag_Name WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Tag_Name", command.Tag_Name);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameters);
                    return (rowEffect > 0) ? ResponseModel<string>.Success("Update Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;

                }
                catch (Exception ex)
                {
                    return ResponseModel<string>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Delete(int id)
        {
            var query = @"DELETE FROM Tags WHERE Id = @Id";
            var queryJunction = @"DELETE FROM Notification_Tag WHERE Tag_Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var rowTagEffect = await connection.ExecuteAsync(queryJunction, parameter, transaction);
                        var rowJuncEffect = await connection.ExecuteAsync(query, parameter, transaction);
                        transaction.Commit();
                        return (rowTagEffect > 0 && rowJuncEffect > 0) ? ResponseModel<string>.Success("Update Detail Successful.") : ResponseModel<string>.Failure("Nothing has changed. There is a problem from your query.")!;
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

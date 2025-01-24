using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;

namespace EV.DataAccess.Repositories
{
    public class JoinGroupRequestRepository
    {
        private readonly DatabaseContext _context;

        public JoinGroupRequestRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<RequestJoinGroupQuery>>> GetByGroupId(Guid id)
        {
            var query = @"SELECT 
                            User_Id, Group_Id, Status, Is_Admin_Request, Admin_Id ,Created_At, Updated_At, Is_Deleted
                          FROM User_Request_Group
                          WHERE Group_Id = @group_Id AND Is_Deleted = 0;";
            var parameter = new DynamicParameters();
            parameter.Add("Group_Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<RequestJoinGroupQuery>(query, parameter);
                    return ResponseModel<IEnumerable<RequestJoinGroupQuery>>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<RequestJoinGroupQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Create(RequestJoinGroupCommand command)
        {
            var query = @"INSERT INTO User_Request_Group (User_Id, Group_Id, Status, Is_Admin_Request, Admin_Id ,Created_At, Updated_At, Is_Deleted) 
                          VALUES
                          (@User_Id, @Group_Id, 'Pending', 0, null ,GETDATE(), GETDATE(), 0)";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Group_Id", command.Group_Id);
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

        public async Task<ResponseModel<string>> AdminCreate(AdminRecommendCommand command)
        {
            var query = @"INSERT INTO User_Request_Group (User_Id, Group_Id, Status, Is_Admin_Request, Admin_Id ,Created_At, Updated_At, Is_Deleted) 
                          VALUES
                          (@User_Id, @Group_Id, 'Pending', 1, @Admin_Id ,GETDATE(), GETDATE(), 0)";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Group_Id", command.Group_Id);
            parameters.Add("Admin_Id", command.Admin_Id);
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

        public async Task<ResponseModel<string>> Edit(ModifyRequestJoinCommand command)
        {
            var query = @"UPDATE User_Request_Group
                          SET Status = @Status, Updated_At = GETDATE()
                          WHERE User_Id = @User_Id AND Group_Id = @Group_Id AND (Status NOT LIKE 'Cancel' OR Status NOT LIKE 'Reject' )";
            var acceptQuery = @"INSERT INTO User_Group (User_Id, Group_Id, Role, Is_Follow, Joined_At, Updated_At ,Is_Deleted)
                              VALUES      
                              (@User_Id, @Group_Id, 'Member', 1 ,GETDATE(), GETDATE() ,0);";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Group_Id", command.Group_Id);
            parameters.Add("Status", command.Status);
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(query, parameters, transaction);
                        if (command.Status == "Accept")
                        {
                            await connection.ExecuteAsync(acceptQuery, parameters, transaction);
                        }
                        transaction.Commit();
                        return ResponseModel<string>.Success("Success.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return ResponseModel<string>.Failure(ex.Message)!;
                    }
                }

            }
        }

        public async Task<ResponseModel<string>> Delete(RequestJoinGroupCommand command)
        {
            var query = "UPDATE User_Request_Group SET Is_Deleted = 1, Updated_At = GETDATE() WHERE User_Id = @User_Id AND Group_Id = @Group_Id;";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Group_Id", command.Group_Id);
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
    }
}

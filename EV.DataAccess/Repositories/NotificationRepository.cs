using Dapper;
using EV.DataAccess.DataConnection;
using EV.Model.Handlers;
using EV.Model.Handlers.Commands;
using EV.Model.Handlers.Queries;
using System.Data;

namespace EV.DataAccess.Repositories
{
    public class NotificationRepository
    {
        private readonly DatabaseContext _context;

        public NotificationRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<IEnumerable<NotificationQuery>>> Get()
        {
            var query = "SELECT Id, User_Id,Title, Description, Noti_Type, Destination_Id, Organization_Name, Started_At, Ended_At FROM Notifications WHERE Is_Deleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<NotificationQuery>(query);
                    return ResponseModel<IEnumerable<NotificationQuery>>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<NotificationQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<IEnumerable<NotificationQuery>>> GetBySearch(string noti_Type)
        {
            var query = "SELECT Id, User_Id, Title, Description, Noti_Type, Destination_Id, Organization_Name, Started_At, Ended_At FROM Notifications WHERE Is_Deleted = 0 AND Noti_Type LIKE @Noti_Type";
            var parameter = new DynamicParameters();
            parameter.Add("Noti_Type", $"%{noti_Type}%");
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<NotificationQuery>(query, parameter);
                    return ResponseModel<IEnumerable<NotificationQuery>>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<IEnumerable<NotificationQuery>>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<NotificationQuery>> GetById(Guid id)
        {
            var query = "SELECT Id, User_Id, Title, Description, Noti_Type, Destination_Id, Organization_Name, Started_At, Ended_At FROM Notifications WHERE Is_Deleted = 0 AND Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QuerySingleAsync<NotificationQuery>(query, parameter);
                    return ResponseModel<NotificationQuery>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<NotificationQuery>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<Guid>> Create(CreateNotificationCommand command)
        {
            var query = @"INSERT INTO Notifications (Id, Created_At, Updated_At, Is_Deleted, User_Id,Title, Description, Noti_Type, Destination_Id, Started_At, Ended_At, Organization_Name)
                        OUTPUT Inserted.Id        
                        VALUES
                        (NEWID(), GETDATE(), GETDATE(), 0, @User_Id, @Title, @Description, @Noti_Type, @Destination_Id, @Started_At, @Ended_At, @Organization_Name);";
            var parameters = new DynamicParameters();
            parameters.Add("User_Id", command.User_Id);
            parameters.Add("Title", command.Title, DbType.String);
            parameters.Add("Description", command.Description, DbType.String);
            parameters.Add("Noti_Type", command.Noti_Type);
            parameters.Add("Destination_Id", command.Destination_Id, DbType.Guid);
            parameters.Add("Started_At", command.Started_At, DbType.DateTime);
            parameters.Add("Ended_At", command.Ended_At, DbType.DateTime);
            parameters.Add("Organization_Name", command.Organization_Name, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QuerySingleAsync<Guid>(query, parameters);
                    return ResponseModel<Guid>.Success(result);

                }
                catch (Exception ex)
                {
                    return ResponseModel<Guid>.Failure(ex.Message)!;
                }
            }
        }

        public async Task<ResponseModel<string>> Edit(Guid id, CreateNotificationCommand command)
        {
            var query = @"UPDATE Notifications
                        SET 
                        Title = @Title, Description = @Description, Destination_Id = @Destination_Id, @Started_At = @Started_At, @Ended_At = @Ended_At, Organization_Name = @Organization_Name
                        WHERE Id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("Title", command.Title, DbType.String);
            parameters.Add("Description", command.Description, DbType.String);
            parameters.Add("Noti_Type", command.Noti_Type, DbType.Int32);
            parameters.Add("Destination_Id", command.Destination_Id, DbType.Guid);
            parameters.Add("Started_At", command.Started_At, DbType.DateTime);
            parameters.Add("Ended_At", command.Ended_At, DbType.DateTime);
            parameters.Add("Organization_Name", command.Organization_Name, DbType.String);
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
            var query = "Update Notifications SET Is_Deleted = 1 WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var rowEffect = await connection.ExecuteAsync(query, parameter);
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

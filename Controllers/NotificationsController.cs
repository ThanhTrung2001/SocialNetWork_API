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
    public class NotificationsController : ControllerBase
    {
        private readonly DapperContext _context;
        public NotificationsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var query = "SELECT Id, Title, Description, Noti_Type, Destination_Id, Organization_Name, Started_At, Ended_At FROM Notifications WHERE Is_Deleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<NotificationQuery>(query);
                    return Ok(ResponseModel<IEnumerable<NotificationQuery>>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<NotificationQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpGet("/type")]
        public async Task<IActionResult> GetNotificationsBySearch([FromQuery] string noti_Type)
        {
            var query = "SELECT Id, Title, Description, Noti_Type, Destination_Id, Organization_Name, Started_At, Ended_At FROM Notifications WHERE Is_Deleted = 0 AND Noti_Type = @Noti_Type;";
            var parameter = new DynamicParameters();
            parameter.Add("Noti_Type", noti_Type);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QueryAsync<NotificationQuery>(query, parameter);
                    return Ok(ResponseModel<IEnumerable<NotificationQuery>>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<IEnumerable<NotificationQuery>>.Failure(ex.Message));
                }
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotification(Guid id)
        {
            var query = "SELECT Id, Title, Description, Noti_Type, Destination_Id, Organization_Name, Started_At, Ended_At FROM Notifications WHERE Is_Deleted = 0 AND Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QuerySingleAsync<NotificationQuery>(query);
                    return Ok(ResponseModel<NotificationQuery>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<NotificationQuery>.Failure(ex.Message));
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNotificationCommand notification)
        {
            var query = @"INSERT INTO Notifications (Id, CreatedAt, UpdatedAt, Is_Deleted, Title, Description, Noti_Type, Destination_Id, Started_At, Ended_At, Organization_Name)
                        OUTPUT Inserted.Id        
                        VALUES
                        (NEWID(), GETDATE(), GETDATE(), 0, @Title, @Description, @Noti_Type, @Destination_Id, @Started_At, @Ended_At, @Organization_Name);";
            var parameters = new DynamicParameters();
            parameters.Add("Title", notification.Title, DbType.String);
            parameters.Add("Description", notification.Description, DbType.String);
            parameters.Add("Noti_Type", notification.Noti_Type, DbType.Int32);
            parameters.Add("Destination_Id", notification.Destination_Id, DbType.Guid);
            parameters.Add("Started_At", notification.Started_At, DbType.DateTime);
            parameters.Add("Ended_At", notification.Ended_At, DbType.DateTime);
            parameters.Add("Organization_Name", notification.Organization_Name, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var result = await connection.QuerySingleAsync<Guid>(query, parameters);
                    return Ok(ResponseModel<Guid>.Success(result));

                }
                catch (Exception ex)
                {
                    return BadRequest(ResponseModel<Guid>.Failure(ex.Message));
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, CreateNotificationCommand notification)
        {
            var query = @"UPDATE Notifications
                        SET 
                        Title = @Title, Description = @Description, Destination_Id = @Destination_Id, @Started_At = @Started_At, @Ended_At = @Ended_At, Organization_Name = @Organization_Name
                        WHERE Id = @Id;";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("Title", notification.Title, DbType.String);
            parameters.Add("Description", notification.Description, DbType.String);
            parameters.Add("Noti_Type", notification.Noti_Type, DbType.Int32);
            parameters.Add("Destination_Id", notification.Destination_Id, DbType.Guid);
            parameters.Add("Started_At", notification.Started_At, DbType.DateTime);
            parameters.Add("Ended_At", notification.Ended_At, DbType.DateTime);
            parameters.Add("Organization_Name", notification.Organization_Name, DbType.String);
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
            var query = "Update Notifications SET Is_Deleted = 1 WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    await connection.ExecuteAsync(query, parameter);
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
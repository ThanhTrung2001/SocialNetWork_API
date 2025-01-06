using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Models.Commands;
using EnVietSocialNetWorkAPI.Models.Queries;
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
        public async Task<IEnumerable<NotificationQuery>> GetNotifications()
        {
            var query = "SELECT Id, Title, Description, NotiType, DestinationId, OrganizationName, StartedAt, EndedAt FROM Notifications WHERE IsDeleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<NotificationQuery>(query);
                return result;
            }
        }

        [HttpGet("/type")]
        public async Task<IEnumerable<NotificationQuery>> GetNotificationsBySearch([FromQuery] int notiType)
        {
            var query = "SELECT Id, Title, Description, NotiType, DestinationId, OrganizationName, StartedAt, EndedAt FROM Notifications WHERE IsDeleted = 0 AND NotiType = @notiType;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<NotificationQuery>(query, new { notiType });
                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<NotificationQuery> GetNotification(Guid id)
        {
            var query = "SELECT Id, Title, Description, NotiType, DestinationId, OrganizationName, StartedAt, EndedAt FROM Notifications WHERE IsDeleted = 0 AND Id = @Id;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<NotificationQuery>(query, new { Id = id });
                return result;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNotificationCommand notification)
        {
            var query = @"INSERT INTO Notifications (Id, CreatedAt, UpdatedAt, IsDeleted, Title, Description, NotiType, DestinationId, StartedAt, EndedAt, OrganizationName)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @Title, @Description, @NotiType, @DestinationId, @StartedAt, @EndedAt, @OrganizationName);";
            var parameter = new DynamicParameters();
            parameter.Add("Title", notification.Title, DbType.String);
            parameter.Add("Description", notification.Description, DbType.String);
            parameter.Add("NotiType", notification.NotiType, DbType.Int32);
            parameter.Add("DestinationId", notification.DestinationId, DbType.Guid);
            parameter.Add("StartedAt", notification.StartedAt, DbType.DateTime);
            parameter.Add("EndedAt", notification.EndedAt, DbType.DateTime);
            parameter.Add("OrganizationName", notification.OrganizationName, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok(notification);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, CreateNotificationCommand notification)
        {
            var query = @"UPDATE Notifications
                        SET 
                        Title = @Title, Description = @Description, DestinationId = @DestinationId, @StartedAt = @StartedAt, @EndedAt = @EndedAt, OrganizationName = @OrganizationName
                        WHERE Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            parameter.Add("Title", notification.Title, DbType.String);
            parameter.Add("Description", notification.Description, DbType.String);
            parameter.Add("NotiType", notification.NotiType, DbType.Int32);
            parameter.Add("DestinationId", notification.DestinationId, DbType.Guid);
            parameter.Add("StartedAt", notification.StartedAt, DbType.DateTime);
            parameter.Add("EndedAt", notification.EndedAt, DbType.DateTime);
            parameter.Add("OrganizationName", notification.OrganizationName, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok(notification);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "Update Notifications SET isDeleted = 1 WHERE Id = @Id";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }
    }
}

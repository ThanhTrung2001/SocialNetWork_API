using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
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
        public async Task<IEnumerable<Notification>> GetNotifications()
        {
            var query = "SELECT * FROM Notifications WHERE IsDeleted = 0;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Notification>(query);
                return result;
            }
        }

        [HttpGet("/type")]
        public async Task<IEnumerable<Notification>> GetNotificationsBySearch([FromQuery] int notiType)
        {
            var query = "SELECT * FROM Notifications WHERE IsDeleted = 0 AND NotiType = @notiType;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Notification>(query, new { notiType });
                return result;
            }
        }

        [HttpGet("{id}")]
        public async Task<Notification> GetNotification(Guid id)
        {
            var query = "SELECT * FROM Notifications WHERE IsDeleted = 0 AND Id = @Id;";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<Notification>(query, new { Id = id });
                return result;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewNotification notification)
        {
            var query = @"INSERT INTO Posts (Id, CreatedAt, UpdatedAt, IsDeleted, Title, Description, NotiType, DestinationId, StartedAt, EndedAt, OrganizeName)
                        VALUES 
                        (NEWID(), GETDATE(), GETDATE(), 0, @Title, @Description), @NotiType, @DestinationId, @StartedAt, @EndedAt, @OrganizeName);";
            var parameter = new DynamicParameters();
            parameter.Add("Title", notification.Title, DbType.String);
            parameter.Add("Description", notification.Description, DbType.String);
            parameter.Add("NotiType", notification.Type, DbType.Int32);
            parameter.Add("DestinationId", notification.DestinationId, DbType.Guid);
            parameter.Add("StartedAt", notification.StartedAt, DbType.DateTime);
            parameter.Add("EndedAt", notification.EndedAt, DbType.DateTime);
            parameter.Add("OrganizeName", notification.OrganizeName, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameter);
                return Ok(notification);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Edit(Guid id, NewNotification notification)
        {
            var query = @"UPDATE Notifications
                        SET 
                        Title = @Title, Description = @Description, DestinationId = @DestinationId, @StartedAt = @StartedAt, @EndedAt = @EndedAt, OrganizeName = @OrganizeName
                        WHERE Id = @Id;";
            var parameter = new DynamicParameters();
            parameter.Add("Id", id, DbType.Guid);
            parameter.Add("Title", notification.Title, DbType.String);
            parameter.Add("Description", notification.Description, DbType.String);
            parameter.Add("NotiType", notification.Type, DbType.Int32);
            parameter.Add("DestinationId", notification.DestinationId, DbType.Guid);
            parameter.Add("StartedAt", notification.StartedAt, DbType.DateTime);
            parameter.Add("EndedAt", notification.EndedAt, DbType.DateTime);
            parameter.Add("OrganizeName", notification.OrganizeName, DbType.String);
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
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }
    }
}

using EV.DataAccess.UnitOfWorks.Interface;
using EV.Model.Handlers.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public NotificationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var result = await _unitOfWork.NotificationRepository.Get();
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("type")]
        public async Task<IActionResult> GetNotificationsBySearch([FromQuery] string noti_Type)
        {
            var result = await _unitOfWork.NotificationRepository.GetBySearch(noti_Type);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _unitOfWork.OrganizationRepository.GetById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNotificationCommand notification)
        {
            var result = await _unitOfWork.NotificationRepository.Create(notification);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, CreateNotificationCommand notification)
        {
            var result = await _unitOfWork.NotificationRepository.Edit(id, notification);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.NotificationRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }
    }
}
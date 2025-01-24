
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
    public class MessagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public MessagesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("chatgroup/{id}")]
        public async Task<IActionResult> GetByChatGroupId(Guid id)
        {
            var result = await _unitOfWork.MessageRepository.GetByChatGroupId(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await _unitOfWork.MessageRepository.GetById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewMessage(CreateMessageCommand message)
        {
            var result = await _unitOfWork.MessageRepository.Create(message);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessage(Guid id, EditMessageCommand message)
        {
            var result = await _unitOfWork.MessageRepository.Edit(id, message);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/pin")]
        public async Task<IActionResult> PinMessage(Guid id)
        {
            var result = await _unitOfWork.MessageRepository.Pin(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.MessageRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }
    }
}
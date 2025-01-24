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
    public class AttachmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AttachmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("post/{id}")]
        public async Task<IActionResult> GetAttachmentsByPostId(Guid id)
        {
            var result = await _unitOfWork.AttachmentRepository.GetByPostId(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("comment/{id}")]
        public async Task<IActionResult> GetAttachmentsByCommentId(Guid id)
        {
            var result = await _unitOfWork.AttachmentRepository.GetByCommentId(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("message/{id}")]
        public async Task<IActionResult> GetAttachmentsByMessageId(Guid id)
        {
            var result = await _unitOfWork.AttachmentRepository.GetByMessageId(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("post/{id}")]
        public async Task<IActionResult> CreatePostAttachment(Guid id, CreateAttachmentListCommand command)
        {
            var result = await _unitOfWork.AttachmentRepository.CreatePostAttachment(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("comment/{id}")]
        public async Task<IActionResult> CreateCommentAttachment(Guid id, CreateAttachmentListCommand command)
        {
            var result = await _unitOfWork.AttachmentRepository.CreateCommentAttachment(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("message/{id}")]
        public async Task<IActionResult> CreateMessageAttachment(Guid id, CreateAttachmentListCommand command)
        {
            var result = await _unitOfWork.AttachmentRepository.CreateMessageAttachment(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditAttachment(Guid id, CreateAttachmentCommand command)
        {
            var result = await _unitOfWork.AttachmentRepository.Edit(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttachment(Guid id)
        {
            var result = await _unitOfWork.AttachmentRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

    }
}

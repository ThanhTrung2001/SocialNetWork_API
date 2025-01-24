
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
    public class ReactsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReactsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #region [Post]

        [HttpGet("post/{post_Id}")]
        public async Task<IActionResult> GetByPostId(Guid post_Id)
        {
            var result = await _unitOfWork.ReactRepository.GetByPostId(post_Id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("post")]
        public async Task<IActionResult> CreateByPost(CreatePostReactCommand react)
        {
            var result = await _unitOfWork.ReactRepository.CreatePostReact(react);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("post/{id}")]
        public async Task<IActionResult> EditPostReact(Guid id, EditReactCommand react)
        {
            var result = await _unitOfWork.ReactRepository.EditPostReact(id, react);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("post/{id}")]
        public async Task<IActionResult> DeletePostReact(Guid id, DeletePostReactCommand command)
        {
            var result = await _unitOfWork.ReactRepository.DeletePostReact(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region [Comment]

        [HttpGet("comment/{comment_Id}")]
        public async Task<IActionResult> GetByCommentId(Guid comment_Id)
        {
            var result = await _unitOfWork.ReactRepository.GetByCommentId(comment_Id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("comment")]
        public async Task<IActionResult> CreateByComment(CreateCommentReactCommand react)
        {
            var result = await _unitOfWork.ReactRepository.CreateCommentReact(react);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("comment/{id}")]
        public async Task<IActionResult> EditCommentReact(Guid id, EditReactCommand react)
        {
            var result = await _unitOfWork.ReactRepository.EditCommentReact(id, react);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("comment/{id}")]
        public async Task<IActionResult> DeleteCommentReact(Guid id, DeleteReactCommand command)
        {
            var result = await _unitOfWork.ReactRepository.DeleteCommentReact(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region [Message]

        [HttpGet("message/{message_Id}")]
        public async Task<IActionResult> GetByMessage_Id(Guid message_Id)
        {
            var result = await _unitOfWork.ReactRepository.GetByMessageId(message_Id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("message")]
        public async Task<IActionResult> CreateByMessage(CreateMessageReactCommand react)
        {
            var result = await _unitOfWork.ReactRepository.CreateMessageReact(react);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("message/{id}")]
        public async Task<IActionResult> EditMessageReact(Guid id, EditReactCommand react)
        {
            var result = await _unitOfWork.ReactRepository.EditMessageReact(id, react);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("message/{id}")]
        public async Task<IActionResult> DeleteMessageReact(Guid id, DeleteReactCommand command)
        {
            var result = await _unitOfWork.ReactRepository.DeleteMessageReact(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        #endregion
    }
}

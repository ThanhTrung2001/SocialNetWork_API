
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
    public class CommentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CommentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("post/{post_Id}")]
        public async Task<IActionResult> GetCommentsByPostId(Guid post_Id)
        {
            var result = await _unitOfWork.CommentRepository.GetByPostId(post_Id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("detail/post/{post_Id}")]
        public async Task<IActionResult> GetCommentDetailsByPostId(Guid post_Id)
        {
            var result = await _unitOfWork.CommentRepository.GetDetailByPostId(post_Id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentByID(Guid id)
        {
            var result = await _unitOfWork.CommentRepository.GetById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/response")]
        public async Task<IActionResult> GetCommentResponseByID(Guid id)
        {
            var result = await _unitOfWork.CommentRepository.GetResponseById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentCommand comment)
        {
            var result = await _unitOfWork.CommentRepository.Create(comment);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditComment(Guid id, EditCommentCommand comment)
        {
            var result = await _unitOfWork.CommentRepository.Edit(id, comment);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.CommentRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }
    }
}
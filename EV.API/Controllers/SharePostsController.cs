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
    public class Share_PostsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public Share_PostsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.SharePostRepository.Get();
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetByUserID(Guid id)
        {
            var result = await _unitOfWork.SharePostRepository.GetByUserId(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _unitOfWork.SharePostRepository.GetById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("post/{post_Id}/users")]
        public async Task<IActionResult> GetShareUsersByPostId(Guid post_Id)
        {
            var result = await _unitOfWork.SharePostRepository.GetShareUsersByPostId(post_Id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSharePost(CreateSharePostCommand share)
        {
            var result = await _unitOfWork.SharePostRepository.Create(share);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSharePost(Guid id, EditSharePostCommand command)
        {
            var result = await _unitOfWork.SharePostRepository.Edit(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.SharePostRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }
    }
}
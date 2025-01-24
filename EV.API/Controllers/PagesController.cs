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
    public class PagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public PagesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.PageRepository.Get();
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var result = await _unitOfWork.PageRepository.Search(name);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await _unitOfWork.PageRepository.GetById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/followers")]
        public async Task<IActionResult> GetUsersFollow(Guid id)
        {
            var result = await _unitOfWork.PageRepository.GetUsersFollow(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/admins")]
        public async Task<IActionResult> GetUsersManage(Guid id)
        {
            var result = await _unitOfWork.PageRepository.GetUsersManage(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/posts")]
        public async Task<IActionResult> GetPosts(Guid id)
        {
            var result = await _unitOfWork.PageRepository.GetPostsById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePageCommand command)
        {
            var result = await _unitOfWork.PageRepository.Create(command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> AddUser(Guid id, ModifyPageUserCommand command)
        {
            var result = await _unitOfWork.PageRepository.AddUser(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, EditPageCommand command)
        {
            var result = await _unitOfWork.PageRepository.Edit(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/users")]
        public async Task<IActionResult> ModifyUser(Guid id, ModifyPageUserCommand command)
        {
            var result = await _unitOfWork.PageRepository.EditUser(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/users/follow")]
        public async Task<IActionResult> ModifyUserFollow(Guid id, ModifyFollowPageCommand command)
        {
            var result = await _unitOfWork.PageRepository.ModifyUserFollow(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.PageRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}/users")]
        public async Task<IActionResult> RemoveUser(Guid id, ModifyFollowPageCommand command)
        {
            var result = await _unitOfWork.PageRepository.DeleteUser(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }
    }
}
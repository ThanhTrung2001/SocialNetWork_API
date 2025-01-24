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
    public class GroupsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.GroupRepository.Get();
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _unitOfWork.GroupRepository.GetById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetBySearch([FromQuery] string name)
        {
            var result = await _unitOfWork.GroupRepository.Search(name);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("user/{user_Id}")]
        public async Task<IActionResult> GetGroupsUserJoined(Guid user_Id)
        {
            var result = await _unitOfWork.GroupRepository.GetGroupsByUserId(user_Id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/posts")]
        public async Task<IActionResult> GetPostsInGroup(Guid id)
        {
            var result = await _unitOfWork.GroupRepository.GetPostsById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupCommand group)
        {
            var result = await _unitOfWork.GroupRepository.Create(group);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, EditGroupCommand group)
        {
            var result = await _unitOfWork.GroupRepository.Edit(id, group);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.GroupRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        #region [User]

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsersInGroup(Guid id)
        {
            var result = await _unitOfWork.GroupRepository.GetUsersById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> AddUsersToGroup(Guid id, ModifyGroupUsersCommand group)
        {
            var result = await _unitOfWork.GroupRepository.AddUser(id, group);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/users")]
        public async Task<IActionResult> EditUserInGroup(Guid id, ModifyGroupUserCommand group)
        {
            var result = await _unitOfWork.GroupRepository.EditUser(id, group);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}/users")]
        public async Task<IActionResult> DeleteUserInGroup(Guid id, DeleteGroupUsersCommand command)
        {
            var result = await _unitOfWork.GroupRepository.DeleteUser(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        #endregion
    }
}

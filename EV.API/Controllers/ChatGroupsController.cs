
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
    public class Chat_GroupsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public Chat_GroupsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.ChatGroupRepository.Get();
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("private/{User_Id1}/{User_Id2}")]
        public async Task<IActionResult> GetPrivateChatGroup(Guid User_Id1, Guid User_Id2)
        {
            var result = await _unitOfWork.ChatGroupRepository.GetPrivateChatGroup(User_Id1, User_Id2);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChatGroupDetail(Guid id)
        {
            var result = await _unitOfWork.ChatGroupRepository.GetById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("user/{user_Id}")]
        public async Task<IActionResult> GetChatGroupsByUserId(Guid user_Id)
        {
            var result = await _unitOfWork.ChatGroupRepository.GetByUserId(user_Id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChatGroup(CreateChatGroupCommand ChatGroup)
        {
            var result = await _unitOfWork.ChatGroupRepository.Create(ChatGroup);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditChatGroup(Guid id, EditChatGroupCommand group)
        {
            var result = await _unitOfWork.ChatGroupRepository.Edit(id, group);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.ChatGroupRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        #region [User]

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsersInGroup(Guid id)
        {
            var result = await _unitOfWork.ChatGroupRepository.GetUsersInChatGroup(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> AddUsersToChatGroup(Guid id, AddUsersToChatGroupCommand group)
        {
            var result = await _unitOfWork.ChatGroupRepository.AddUserToChatGroup(id, group);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/users")]
        public async Task<IActionResult> EditUserInChatGroup(Guid id, ModifyChatGroupUserCommand group)
        {
            var result = await _unitOfWork.ChatGroupRepository.EditUserInChatGroup(id, group);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}/users")]
        public async Task<IActionResult> DeleteUsersFromChatGroup(Guid id, DeleteChatGroupUsersCommand group)
        {
            var result = await _unitOfWork.ChatGroupRepository.DeleteUserFromChatGroup(id, group);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/users/follow")]
        public async Task<IActionResult> ModifyUserChatNotification(Guid id, ChangeNotificationCommand command)
        {
            var result = await _unitOfWork.ChatGroupRepository.ModifyUserChatNotification(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);

        }

        #endregion











    }
}

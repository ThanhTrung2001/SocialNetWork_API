//using EnVietSocialNetWorkAPI.Services.Email;
//using EnVietSocialNetWorkAPI.Services.Email.Model;
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
    public class UsersController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.UserRepository.GetAll();
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetBySearch([FromQuery] string name)
        {
            var result = await _unitOfWork.UserRepository.Search(name);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await _unitOfWork.UserRepository.GetByID(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserCommand user)
        {
            var result = await _unitOfWork.UserRepository.Create(user);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, EditUserCommand command)
        {
            var result = await _unitOfWork.UserRepository.Edit(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/detail")]
        public async Task<IActionResult> EditUserDetail(Guid id, EditUserDetailCommand command)
        {
            var result = await _unitOfWork.UserRepository.EditDetail(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/password")]
        public async Task<IActionResult> ChangePassword(Guid id, ChangeUserPasswordCommand command)
        {
            var result = await _unitOfWork.UserRepository.ChangePassword(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.UserRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }


    }
}

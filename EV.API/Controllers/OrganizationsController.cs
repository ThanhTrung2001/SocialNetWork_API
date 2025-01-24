using EV.DataAccess.UnitOfWorks.Interface;
using EV.Model.Handlers.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrganizationsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.OrganizationRepository.Get();
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _unitOfWork.OrganizationRepository.GetById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganizeNodeCommand node)
        {
            var result = await _unitOfWork.OrganizationRepository.Create(node);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, CreateOrganizeNodeCommand node)
        {
            var result = await _unitOfWork.OrganizationRepository.Edit(id, node);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.OrganizationRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetUsersByID(Guid id)
        {
            var result = await _unitOfWork.OrganizationRepository.GetUsersByOrganizationId(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/users")]
        public async Task<IActionResult> CreateUserInOrganization(Guid id, CreateOrganizationUserCommand command)
        {
            var result = await _unitOfWork.OrganizationRepository.CreateUser(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/users")]
        public async Task<IActionResult> EditUserInOrganization(Guid id, EditOrganizationUserCommand command)
        {
            var result = await _unitOfWork.OrganizationRepository.EditUser(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}/users")]
        public async Task<IActionResult> DeleteUserInOrganization(Guid id, DeleteOrganizationUserCommand command)
        {
            var result = await _unitOfWork.OrganizationRepository.DeleteUser(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }
    }
}

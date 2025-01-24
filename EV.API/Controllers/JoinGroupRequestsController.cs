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
    public class JoinGroupRequestsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public JoinGroupRequestsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("group/{group_Id}")]
        public async Task<IActionResult> GetByGroupId(Guid group_Id)
        {
            var result = await _unitOfWork.JoinGroupRequestRepository.GetByGroupId(group_Id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(RequestJoinGroupCommand command)
        {
            var result = await _unitOfWork.JoinGroupRequestRepository.Create(command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("admin")]
        public async Task<IActionResult> AdminRecommendUser(AdminRecommendCommand command)
        {
            var result = await _unitOfWork.JoinGroupRequestRepository.AdminCreate(command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> ModifyRequest(ModifyRequestJoinCommand command)
        {
            var result = await _unitOfWork.JoinGroupRequestRepository.Edit(command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRequest(RequestJoinGroupCommand command)
        {
            var result = await _unitOfWork.JoinGroupRequestRepository.Delete(command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }
    }
}

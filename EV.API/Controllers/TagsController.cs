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
    public class TagsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public TagsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.TagRepository.GetAll();
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTagCommand command)
        {
            var result = await _unitOfWork.TagRepository.Create(command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, CreateTagCommand command)
        {
            var result = await _unitOfWork.TagRepository.Edit(id, command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _unitOfWork.TagRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

    }
}

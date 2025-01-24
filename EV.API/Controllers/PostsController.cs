using EV.DataAccess.UnitOfWorks.Interface;
using EV.Model.Handlers.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/<PostController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _unitOfWork.PostRepository.Get();
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserPostByUserID(Guid id)
        {
            var result = await _unitOfWork.PostRepository.GetByUserId(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(Guid id)
        {
            var result = await _unitOfWork.PostRepository.GetById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        // POST api/<PostController>
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostRequest request)
        {
            var result = await _unitOfWork.PostRepository.Create(request);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        // PUT api/<PostController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, EditPostRequest request)
        {
            var result = await _unitOfWork.PostRepository.Edit(id, request);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.PostRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }
    }
}
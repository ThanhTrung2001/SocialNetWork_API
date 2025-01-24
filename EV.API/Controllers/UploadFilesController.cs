using EV.Common.Services.UploadFile.Interfaces;
using EV.Model.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFilesController : ControllerBase
    {
        private readonly IUploadFiles _service;
        public UploadFilesController(IUploadFiles service)
        {
            _service = service;
        }


        [HttpGet("connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                bool test = _service.TestConnection();
                return Ok(test);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpGet("directory/{id}")]
        public async Task<IActionResult> ListFiles(Guid? id, [FromQuery] string type)
        {
            try
            {
                var result = await _service.ListFilesInAlbum(id, type);
                return Ok(ResponseModel<IEnumerable<string>>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<string>>.Failure(ex.Message));
            }
        }

        [HttpPost("upload/{id}")]
        public async Task<IActionResult> UploadFiles(Guid id, List<IFormFile> files, [FromQuery] string type)
        {
            try
            {
                var result = await _service.UploadFiles(type, files, id);
                return Ok(ResponseModel<IEnumerable<string>>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<string>>.Failure(ex.Message));
            }
        }

        [HttpPost("download")]
        public async Task<IActionResult> DownloadFiles(List<string> files)
        {
            try
            {
                var result = await _service.DownloadFiles(files);
                return Ok(ResponseModel<IEnumerable<string>>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<string>>.Failure(ex.Message));
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFiles(List<string> files)
        {
            try
            {
                var result = await _service.DeleteFiles(files);
                return Ok(ResponseModel<string>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<string>.Failure(ex.Message));
            }
        }
    }
}

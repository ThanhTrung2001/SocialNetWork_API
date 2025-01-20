using EnVietSocialNetWorkAPI.Models;
using EnVietSocialNetWorkAPI.Services.Upload;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFilesController : ControllerBase
    {
        private readonly IUploadFileService _service;
        public UploadFilesController(IUploadFileService service)
        {
            _service = service;
        }


        //Attachment Upload action
        [HttpGet("directory/{id}")]
        public async Task<IActionResult> ListFiles(Guid id)
        {
            try
            {
                var result = await _service.ListFilesInAlbum(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<string>>.Failure(ex.Message));
            }
        }

        [HttpPost("upload/{id}")]
        public async Task<IActionResult> UploadFiles(Guid id, string type, List<IFormFile> files)
        {
            try
            {
                var result = await _service.UploadFiles(type, files, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<string>>.Failure(ex.Message));
            }
        }

        [HttpDelete("download")]
        public async Task<IActionResult> DownloadFiles(List<string> files)
        {
            try
            {
                var result = await _service.DownloadFiles(files);
                return Ok(result);
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseModel<IEnumerable<string>>.Failure(ex.Message));
            }
        }
    }
}

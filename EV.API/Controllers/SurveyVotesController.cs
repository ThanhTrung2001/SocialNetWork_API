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
    public class SurveyVotesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public SurveyVotesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("option/{surveyItem_Id}")]
        public async Task<IActionResult> GetByOptionId(Guid surveyItem_Id)
        {
            var result = await _unitOfWork.SurveyVoteRepository.GetBySurveyItemID(surveyItem_Id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateByOptionId(CreateSurveyVoteCommand command)
        {
            var result = await _unitOfWork.SurveyVoteRepository.Create(command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("option/{surveyItem_Id}")]
        public async Task<IActionResult> Delete(CreateSurveyVoteCommand command)
        {
            var result = await _unitOfWork.SurveyVoteRepository.Delete(command);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

    }
}

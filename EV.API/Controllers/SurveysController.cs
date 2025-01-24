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
    public class SurveysController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public SurveysController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("post/{id}")]
        public async Task<IActionResult> GetByPostId(Guid id)
        {
            var result = await _unitOfWork.SurveyRepository.GetByPostId(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _unitOfWork.SurveyRepository.GetById(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateByPostId(CreateSurveyCommand survey)
        {
            var result = await _unitOfWork.SurveyRepository.Create(survey);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/item")]
        public async Task<IActionResult> AddNewSurveyItem(Guid id, EditSurveyItemCommand surveyItem)
        {
            var result = await _unitOfWork.SurveyItemRepository.Create(id, surveyItem);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSurvey(Guid id, EditSurveyCommand survey)
        {
            var result = await _unitOfWork.SurveyRepository.Edit(id, survey);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpPut("item/{id}")]
        public async Task<IActionResult> UpdateSurveyItem(Guid id, EditSurveyItemCommand surveyItem)
        {
            var result = await _unitOfWork.SurveyItemRepository.Edit(id, surveyItem);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitOfWork.SurveyRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("item/{id}")]
        public async Task<IActionResult> DeleteSurveyItem(Guid id)
        {
            var result = await _unitOfWork.SurveyItemRepository.Delete(id);
            return result.IsSuccessed ? Ok(result) : BadRequest(result);
        }
    }
}

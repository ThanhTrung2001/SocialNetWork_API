//using EnVietSocialNetWorkAPI.Old.Repositories.Interface;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace EnVietSocialNetWorkAPI.Old
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CompanyController : ControllerBase
//    {
//        private readonly ICompanyRepository _repository;
//        public CompanyController(ICompanyRepository repository)
//        {
//            _repository = repository;
//        }

//        //GET: api/Company
//        [HttpGet]
//        public async Task<IEnumerable<Company>> GetALl()
//        {
//            return await _repository.GetAll();
//        }
//        //POST: api/Company
//        [HttpPost]
//        public async Task Add(CreateCompany company)
//        {
//            try
//            {
//                //await _unitOfWork.Begin();
//                //var repository = _unitOfWork.GetRepository<Company>()
//                await _repository.AddCompany(company);
//                //await _unitOfWork.Commit();
//            }
//            catch (Exception ex)
//            {
//                //await _unitOfWork.Rollback();
//                throw ex;
//            }


//        }

//    }
//}

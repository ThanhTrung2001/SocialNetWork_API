//using EnVietSocialNetWorkAPI.Old.UnitOfWork.Interface;
//using Microsoft.AspNetCore.Mvc;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace EnVietSocialNetWorkAPI.Old
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EmployeeController : ControllerBase
//    {
//        private readonly IUnitOfWork _unitOfWork;

//        public EmployeeController(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }

//        // GET: api/<EmployeeController>
//        [HttpGet]
//        public Task<IEnumerable<Employee>> GetAll()
//        {
//            return _unitOfWork.GetRepository<Employee>().GetAll();
//        }

//        // GET api/<EmployeeController>/5
//        [HttpGet("{id}")]
//        public string Get(int id)
//        {
//            return "value";
//        }

//        // POST api/<EmployeeController>
//        [HttpPost]
//        public void Post([FromBody] string value)
//        {
//        }

//        // PUT api/<EmployeeController>/5
//        [HttpPut("{id}")]
//        public void Put(int id, [FromBody] string value)
//        {
//        }

//        // DELETE api/<EmployeeController>/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }
//    }
//}

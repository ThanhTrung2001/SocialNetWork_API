using EnVietSocialNetWorkAPI.DataConnection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediasController : ControllerBase
    {

        private readonly DapperContext _context;

        public MediasController(DapperContext context)
        {
            _context = context;
        }

    }
}

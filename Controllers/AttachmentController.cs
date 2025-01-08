using EnVietSocialNetWorkAPI.DataConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnVietSocialNetWorkAPI.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly DapperContext _context;
        public AttachmentController(DapperContext context)
        {
            _context = context;
        }

    }
}

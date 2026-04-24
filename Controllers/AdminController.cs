using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace WebApiTestBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="Admin,User")]
    [Authorize(Policy= "AdminOnly")]
    public class AdminController : ControllerBase
    {
        [HttpGet]

        [OutputCache(Duration =20)]
        public IActionResult Get()
        {
            var role =  User.IsInRole("Admin");
            return Ok("Only Admin can access");
        }
    }
}

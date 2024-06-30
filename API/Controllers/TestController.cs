using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /**
     * Refer to this controller when you have trouble making role based authenticated routes
     */
    [Route("api/[Controller]")]
    public class TestController : ControllerBase
    {

        public TestController()
        {
        }

        [HttpGet("admin")] // Route only for authenticated admins
        //[Authorize(Policy = "AdminOnly")]
        public IActionResult Index()
        {

            return Ok(new
            {
                Message ="Hellow"
            });
        }

        [HttpGet("user")] // Route only for patients
        //[Authorize(Policy = "PatientOnly")]
        public IActionResult Patient() 
        {
            return Ok(new
            {
                Message = "This route is for patients"
            });
        }

        [HttpGet("all")] // Route for all authenticated users
        public IActionResult All() 
        {
            return Ok(new
            {
                Name = "sdf",
                Age = 20
            });
        }

        [HttpGet("forAll")]
        public IActionResult ForAll() 
        {
            return Ok(new
            {
                Message = "Working for everyone"
            });
        }
    }
}

using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    public class PatientController : ControllerBase
    {
        ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var patients = _context.Patients;

            return Ok(patients);
        }
    }
}

using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Index()
        {
            var patients = await _context.Patients.Include( p => p.Admin)
                .ToListAsync();

            return Ok(patients);
        }

        [Route("latest")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Latest()
        {
            var patients = await _context.Patients.
                OrderBy(p => p.RegisteredDate)
                .Take(5)
                .ToListAsync();

            return Ok();
        }
    }

}

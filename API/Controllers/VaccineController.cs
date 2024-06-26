using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class VaccineController : Controller
    {
        ApplicationDbContext _context;

        public VaccineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/vaccine
        [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "DoctorOnly")]
        public async Task<IActionResult> Index()
        {
            var vaccines = await _context.Vaccines.Include(v => v.VaccineBrands).ToListAsync();

            var results = new List<Object>();
            vaccines.ForEach(v =>
            {
                var result = new
                {
                    v.Id, v.Name,
                    VaccineBrands = new List<Object>(),
                };

                v.VaccineBrands.ForEach(vb =>
                {
                    var brand = new
                    {
                        vb.Id,
                        vb.BrandName
                    };

                    result.VaccineBrands.Add(brand);
                });

                results.Add(result);
            });

            return Ok(results);
        }
    }
}

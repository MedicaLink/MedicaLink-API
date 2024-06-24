using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class VaccineBrandController : Controller
    {
        ApplicationDbContext _context;

        public VaccineBrandController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/vaccineBrand
        public async Task<IActionResult> Index()
        {
            var vaccineBrands = await _context.VaccineBrands.Include(vb => vb.Vaccine).ToListAsync();

            var results = new List<Object>();
            vaccineBrands.ForEach(vb => {
                var result = new
                {
                    vb.Id,
                    vb.BrandName,
                    Vaccine = new
                    {
                        vb.Vaccine.Id,
                        vb.Vaccine.Name,
                    }
                };

                results.Add(result);
            });

            return Ok(results);
        }
    }
}

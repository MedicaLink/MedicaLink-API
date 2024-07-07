using API.Data;
using API.Models;
using API.Models.FormModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class VaccinationsController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public VaccinationsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET
    public async Task<IActionResult> Index([FromQuery] VaccinationModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var patientId = model.PatientId;

        var vaccinations = await _context.Vaccinations
            .Where(v => v.PatientId == patientId)
            .Include(v => v.Hospital)
            .Include(v => v.VaccineBrand)
            .ThenInclude(vb => vb.Vaccine)
            .OrderByDescending(v => v.DateOfVaccination)
            .Take(6)
            .ToListAsync();

        var results = new List<Object>();

        vaccinations.ForEach(v =>
        {
            var result = new
            {
                v.Id,
                v.DateOfVaccination,
                v.Dose,
                VaccineBrand = new
                {
                    v.VaccineBrand.Id,
                    v.VaccineBrand.BrandName,
                    Vaccine = new
                    {
                        v.VaccineBrand.Vaccine.Id,
                        v.VaccineBrand.Vaccine.Name
                    }
                },
                Hospital = new
                {
                    v.Hospital.Id,
                    v.Hospital.Name,
                    v.Hospital.LogoImage
                }
            };

            results.Add(result);
        });

        return Ok(results);
    }

    [Route("search")]
    public async Task<IActionResult> Search([FromQuery] VaccinationSearchModel model)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);

        string searchQuery = model.Query;
        string searchType = model.Type;
        int patientId = model.PatientId;

        IQueryable<Vaccination> query = _context.Vaccinations.Where(v => v.PatientId == patientId);

        if (!searchQuery.IsNullOrEmpty()) 
        {
            if (searchType == "Location")
            {
                query = query.Where(v => EF.Functions.Like(v.Hospital.Name, $"%{searchQuery}%"));
            }
            else if (searchType == "All")
            {
                query = query.Where(v => EF.Functions.Like(v.VaccineBrand.Vaccine.Name, $"%{searchQuery}%") || EF.Functions.Like(v.Hospital.Name, $"%{searchQuery}%"));
            }
            else
            {
                query = query.Where(v => EF.Functions.Like(v.VaccineBrand.Vaccine.Name, $"%{searchQuery}%"));
            }
        }

        var vaccinations = await query.Include(v => v.Hospital)
            .Include(v => v.VaccineBrand)
            .ThenInclude(vb => vb.Vaccine)
            .OrderByDescending(v => v.DateOfVaccination)
            .ToListAsync();

        int adminHospitalId = 1; // This should be retireved from the JWT

        var results = new List<Object>();
        vaccinations.ForEach(v =>
        {
            var result = new
            {
                v.Id,
                v.DateOfVaccination,
                v.Dose,
                VaccineBrand = new
                {
                    v.VaccineBrand.Id,
                    v.VaccineBrand.BrandName,
                    Vaccine = new
                    {
                        v.VaccineBrand.Vaccine.Id,
                        v.VaccineBrand.Vaccine.Name
                    }
                },
                Hospital = new
                {
                    v.Hospital.Id,
                    v.Hospital.Name,
                    v.Hospital.LogoImage
                },
                IsEditable = v.Hospital.Id == adminHospitalId // Check wether the user can edit the record
            };

            results.Add(result);
        });

        return Ok(results);
    }

    [Authorize(Policy = "AdminOnly")]
    [Authorize(Policy = "DoctorOnly")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VaccinationModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Fetch the admin
        var userId = User.FindFirstValue(ClaimTypes.PrimarySid);
        if (userId == null) return BadRequest("User lacks neccessary credentials");

        int uId = int.Parse(userId);
        var admins = await _context.Admins.Where(a => a.Id == uId).ToListAsync();
        if (admins.IsNullOrEmpty()) return BadRequest("User lacks neccessary credentials");
        var admin = admins[0];

        var v = new Vaccination
        {
            PatientId = model.PatientId,
            HospitalId = admin.HospitalId,//model.HospitalId,
            VaccineBrandId = model.VaccineBrandId,
            DateOfVaccination = model.DateOfVaccination,
            Dose = model.Dose
        };

        _context.Vaccinations.Add(v);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            request = "success",
            message = "Vaccination added",
        });
    }

    [Authorize(Policy = "AdminOnly")]
    [Authorize(Policy = "DoctorOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] VaccinationModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingVaccination = await _context.Vaccinations.FindAsync(id);

        // Makes changes
        existingVaccination.VaccineBrandId = model.VaccineBrandId;
        existingVaccination.DateOfVaccination = model.DateOfVaccination;
        existingVaccination.Dose = model.Dose;

        await _context.SaveChangesAsync(); // Saves the changes in the database

        return Ok(new
        {
            request = "success",
            message = "Vaccination updated",
        });
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var vaccination = await _context.Vaccinations.FindAsync(id);

        if (vaccination == null)
        {
            return NotFound();
        }

        _context.Vaccinations.Remove(vaccination);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
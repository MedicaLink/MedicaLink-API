using API.Data;
using API.Models;
using API.Models.FormModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
public class VaccinationsController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public VaccinationsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // [Authorize]
    // GET
    public async Task<IActionResult> Index([FromBody] VaccinationModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var patientId = model.PatientId;

        var vaccinations = await _context.Vaccinations
            .Where(v => v.PatientId == patientId)
            .Include(v => v.VaccineBrand)
            .ToListAsync();
            
        return Ok(vaccinations);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VaccinationModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var v = new Vaccination
        {
            PatientId = model.PatientId,
            HospitalId = model.HospitalId,
            VaccineBrandId = model.VaccineBrandId,
            DateOfVaccination = model.DateOfVaccination,
            Dose = model.Dose
        };

        _context.Vaccinations.Add(v);
        await _context.SaveChangesAsync();

        return Ok(model);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] VaccinationModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingVaccination = await _context.Vaccinations.FindAsync(id);

        if (existingVaccination == null)
        {
            return NotFound();
        }
        
        existingVaccination.PatientId = model.PatientId;
        existingVaccination.HospitalId = model.HospitalId;
        existingVaccination.VaccineBrandId = model.VaccineBrandId;
        existingVaccination.DateOfVaccination = model.DateOfVaccination;
        existingVaccination.Dose = model.Dose;

        await _context.SaveChangesAsync();

        return Ok(existingVaccination);
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
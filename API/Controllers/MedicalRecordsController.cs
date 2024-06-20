using API.Data;
using API.Models;
using API.Models.FormModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
public class MedicalRecordsController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public MedicalRecordsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // POST: api/MedicalRecords
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MedicalRecordsModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var medicalRecord = new MedicalRecord
        {
            PatientId = model.PatientId,
            AdminId = model.AdminId,
            RecordType = model.RecordType,
            Description = model.Description,
            Date = model.Date,
            FilePath = model.FilePath
        };

        _context.MedicalRecords.Add(medicalRecord);
        await _context.SaveChangesAsync();

        return Ok(medicalRecord);
    }
    
    // GET: api/MedicalRecords
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] MedicalRecordsModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var patientId = model.PatientId;

        var medicalRecords = await _context.MedicalRecords
            .Where(m => m.PatientId == patientId)
            .ToListAsync();
            
        return Ok(medicalRecords);
    }
    
    // PUT: api/MedicalRecords/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] MedicalRecordsModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingRecord = await _context.MedicalRecords.FindAsync(id);

        if (existingRecord == null)
        {
            return NotFound();
        }

        existingRecord.PatientId = model.PatientId;
        existingRecord.AdminId = model.AdminId;
        existingRecord.RecordType = model.RecordType;
        existingRecord.Description = model.Description;
        existingRecord.Date = model.Date;
        existingRecord.FilePath = model.FilePath;

        await _context.SaveChangesAsync();

        return Ok(existingRecord);
    }

    // DELETE: api/MedicalRecords/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var medicalRecord = await _context.MedicalRecords.FindAsync(id);

        if (medicalRecord == null)
        {
            return NotFound();
        }

        _context.MedicalRecords.Remove(medicalRecord);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
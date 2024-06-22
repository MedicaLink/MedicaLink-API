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
    
    // Create a new medical record - POST: api/MedicalRecords
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
    
    // List all medical records - GET: api/MedicalRecords
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] MedicalRecordModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        int patientId = model.PatientId;

        var medicalRecords = await _context.MedicalRecords
            .Where(m => m.PatientId == patientId)
            .Include(m => m.Admin)
            .ThenInclude(a => a.Hospital)
            .OrderByDescending(m => m.Date)
            .Take(6)
            .ToListAsync();

        var results = new List<Object>();

        medicalRecords.ForEach(m =>
        {
            var result = new
            {
                m.Id,
                m.RecordType, m.Description,
                m.Date, m.FilePath,
                Admin = new
                {
                    m.Admin.Id,
                    m.Admin.Name,
                    Hospital = new
                    {
                        m.Admin.Hospital.Id,
                        m.Admin.Name,
                    }
                }
            };

            results.Add(result);
        });

        return Ok(results);
    }
    
    // Update a medical record - PUT: api/MedicalRecords/{id}
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

    // Delete a medical record - DELETE: api/MedicalRecords/{id}
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
    
    // Search for a medical record - GET: api/MedicalRecords/search
    [Route("search")]
    public async Task<IActionResult> Search([FromQuery] MedicalRecordSearchModel model)
    {
        if(!ModelState.IsValid) return BadRequest(model);

        int patientId = model.PatientId;
        string searchQuery = model.Query;
        string searchType = model.Type;

        IQueryable<MedicalRecord> query = _context.MedicalRecords;

        if(searchType == "Location")
        {
            query = query.Where(m => EF.Functions.Like(m.Admin.Hospital.Name, searchQuery));
        }
        else if(searchType == "All")
        {
            query = query.
                Where(m => EF.Functions.Like(m.RecordType, searchQuery) || EF.Functions.Like(m.Admin.Hospital.Name, searchQuery));
        }
        else{
            query = query.
                Where(m => EF.Functions.Like(m.Admin.Hospital.Name, searchQuery));
        }
            
        var medicalRecords = await query
            .Include(m => m.Admin)
            .ThenInclude(a => a.Hospital)
            .ToListAsync();

        var results = new List<Object>();

        medicalRecords.ForEach(m =>
        {
            var result = new
            {
                m.Id, m.FilePath, m.Date, m.Description,
                Admin = new
                {
                    m.Admin.Id,
                    m.Admin.Name,
                    Hospital = new
                    {
                        m.Admin.Hospital.Id,
                        m.Admin.Hospital.Name,
                    }
                }
            };

            results.Add(result);
        });

        return Ok(results);
    }
}
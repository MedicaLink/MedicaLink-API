using API.Data;
using API.Models.FormModels;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class MedicalRecordController : Controller
    {
        ApplicationDbContext _context;

        public MedicalRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

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
}

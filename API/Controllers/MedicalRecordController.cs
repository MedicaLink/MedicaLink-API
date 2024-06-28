using API.Data;
using API.Models.FormModels;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class MedicalRecordController : Controller
    {
        ApplicationDbContext _context;

        public MedicalRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MedicalRecord
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

        // GET: api/MedicalRecord/search
        [Route("search")]
        public async Task<IActionResult> Search([FromQuery] MedicalRecordSearchModel model)
        {
            if(!ModelState.IsValid) return BadRequest(model);

            int patientId = model.PatientId;
            string searchQuery = model.Query;
            string searchType = model.Type;

            IQueryable<MedicalRecord> query = _context.MedicalRecords.Where(m => m.PatientId == patientId);

            if (!searchQuery.IsNullOrEmpty())
            {
                if (searchType == "Location")
                {
                    query = query.Where(m => EF.Functions.Like(m.Admin.Hospital.Name, $"%{searchQuery}%"));
                }
                else if (searchType == "All")
                {
                    query = query.
                        Where(m => EF.Functions.Like(m.RecordType, $"%{searchQuery}%") || EF.Functions.Like(m.Admin.Hospital.Name, $"%{searchQuery}%"));
                }
                else
                {
                    query = query.
                        Where(m => EF.Functions.Like(m.RecordType, $"%{searchQuery}%"));
                }
            }
            
            var medicalRecords = await query
                .Include(m => m.Admin)
                .ThenInclude(a => a.Hospital)
                .ToListAsync();

            // To check whether the current user has the ability to edit this record
            var userId = User.FindFirstValue(ClaimTypes.PrimarySid);
            if (userId == null) return BadRequest("User doesn't have the neccessary access credentials");
            
            var uID = int.Parse(userId);


            var results = new List<Object>();
            medicalRecords.ForEach(m =>
            {
                var result = new
                {
                    m.Id, m.RecordType, m.FilePath, m.Date, m.Description,
                    Admin = new
                    {
                        m.Admin.Id,
                        m.Admin.Name,
                        Hospital = new
                        {
                            m.Admin.Hospital.Id,
                            m.Admin.Hospital.Name,
                        }
                    },
                    IsEditable = m.AdminId == uID // Indicates wether the user can edit the record
                };

                results.Add(result);
            });

            return Ok(results);
        }

        // POST: api/MedicalRecord
        [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "DoctorOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] MedicalRecordsModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.PrimarySid);
            if(userId == null) return BadRequest("User doesn't have permission, the user Id is missing");

            var medicalRecord = new MedicalRecord
            {
                PatientId = model.PatientId,
                AdminId = int.Parse(userId),
                RecordType = model.RecordType,
                Description = model.Description,
                Date = model.Date,
                //FilePath = model.FilePath
            };

            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            return Ok(medicalRecord);
        }

        // PUT: api/MedicalRecord/{id}
        [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "DoctorOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] MedicalRecordsModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRecord = await _context.MedicalRecords.FindAsync(id);

            if (existingRecord == null) return NotFound();

            if (! User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.PrimarySid);
                if (userId == null) return BadRequest("This doctor doesn't have authority");
                if (existingRecord.AdminId != int.Parse(userId)) return BadRequest("This doctor doesn't have edit access to this vaccination");
            }

            existingRecord.RecordType = model.RecordType;
            existingRecord.Description = model.Description;
            existingRecord.Date = model.Date;
            //existingRecord.FilePath = model.FilePath;

            await _context.SaveChangesAsync();

            return Ok(existingRecord);
        }

        // DELETE: api/MedicalRecord/{id}
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
}
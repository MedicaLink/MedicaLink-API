using API.Data;
using API.Models;
using API.Models.FormModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;

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

        [HttpGet("{patientId:int}")]
        public async Task<IActionResult> Single(int patientId)
        {
            var p = _context.Patients
                .Include( p => p.Admin)
                .ThenInclude(a => a.Hospital)
                .SingleOrDefault( p => p.Id == patientId);

            var result = new
            {
                p.Id, p.Name,
                Age = p.getAge(),
                p.Nic, p.BloodGroup,
                p.DateOfBirth, p.Gender,
                p.Height, p.Weight,
                p.RegisteredDate, p.ProfileImage,
                Admin = new
                {
                    p.Admin.Name,
                    Hospital = new
                    {
                        p.Admin.Hospital.Name,
                    }
                }
            };

            return Ok(result);
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

        [Route("search")]
        /*[Authorize(Policy = "AdminOnly")]*/
        public async Task<IActionResult> Search([FromQuery] PatientSearchModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string searchQuery = model.Query;
            string type = model.Type;

            Console.WriteLine(searchQuery);

            IQueryable<Patient> query = _context.Patients;

            if (!searchQuery.IsNullOrEmpty())
            {
                if (type == "Name")
                {
                    query = query.Where(p => EF.Functions.Like(p.Name, $"%{searchQuery}%"));
                }
                else if (type == "Hospital")
                {
                    query = query.Include(p => p.Admin.Hospital)
                            .Where(p => EF.Functions.Like(p.Admin.Hospital.Name, $"%{searchQuery}%"));
                }
                else
                {
                    query = query.Where(p => EF.Functions.Like(p.Nic, $"%{searchQuery}%"));
                }
            }

            var patients = await query
                .Include(p => p.Admin).ThenInclude(a => a.Hospital).ToListAsync();

            var results = new List<Object>();
            
            patients.ForEach(p => {
                var result = new
                {
                    p.Id,
                    p.Name,
                    p.Nic,
                    p.RegisteredDate,
                    p.ProfileImage,
                    Admin = new
                    {
                        p.Admin.Name,
                        Hospital = new
                        {
                            p.Admin.Hospital.Name,
                        }
                    }
                };

                results.Add(result);
            });

            return Ok(results);
        }
    }

}

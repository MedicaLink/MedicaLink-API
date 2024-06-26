using API.Data;
using API.Models;
using API.Models.FormModels;
using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IPasswordHasher<Patient> _passwordHasher;

        public PatientController(ApplicationDbContext context, IPasswordHasher<Patient> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
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
                p.Address,
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
        //[Authorize(Policy = "DoctorOnly")]
        public async Task<IActionResult> Latest()
        {
            var patients = await _context.Patients
                .Include(p => p.Admin)
                .ThenInclude(a => a.Hospital)
                .OrderBy(p => p.RegisteredDate)
                .Take(5)
                .ToListAsync();

            var results = new List<Object>();

            patients.ForEach(p =>
            {
                var result = new
                {
                    p.Id,
                    p.Name,
                    Age = p.getAge(),
                    p.Nic,
                    p.BloodGroup,
                    p.DateOfBirth,
                    p.Gender,
                    p.Height,
                    p.Weight,
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

        [Route("search")]
        [Authorize(Policy = "AdminOnly")]
        //[Authorize(Policy = "DoctorOnly")]
        public async Task<IActionResult> Search([FromQuery] PatientSearchModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string searchQuery = model.Query;
            string type = model.Type;

            IQueryable<Patient> query = _context.Patients.Include(p => p.Admin).ThenInclude(a => a.Hospital);

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
                .ToListAsync();

            var results = patients.Select(p => new
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
                },
                Relavance = searchQuery.IsNullOrEmpty()? 0 : type switch
                {
                    "Name" => GetRelevanceMark(p.Name, searchQuery),
                    "Hospital" => GetRelevanceMark(p.Admin.Hospital.Name, searchQuery),
                    _ => GetRelevanceMark(p.Nic, searchQuery)
                }
            })
                .OrderBy(p => p.Relavance)
                .ToList();

            return Ok(results);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        //[Authorize(Policy = "DoctorOnly")]
        public async Task<IActionResult> create([FromForm] PatientModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Patient patient = new Patient()
            {
                Name = model.Name,
                DateOfBirth = model.dateOfBirth,
                Height = model.Height,
                Weight = model.Weight,
                Nic = model.Nic,
                RegisteredBy = 1,
                Gender = model.Gender switch
                {
                    "Female" => Gender.Female,
                    _=> Gender.Male
                },
                BloodGroup = model.BloodGroup switch
                {
                    "A" => BloodGroup.A,
                    "B" => BloodGroup.B,
                    _=> BloodGroup.O
                },
                Address = model.Address,
            };

            // Generate a password
            var faker = new Bogus.Faker();
            var password = faker.Random.Replace("########"); // This should be sent along with the response
            var hashedPassword = _passwordHasher.HashPassword(patient, password);

            patient.Password = hashedPassword;

            _context.Patients.Add(patient);
            var result = await _context.SaveChangesAsync();

            if(result > 1)
            {
                return BadRequest("Could not add patient");
            }

            return Ok(new
            {
                Type = "Success",
                Message = "Patient added successfully",
                password
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        //[Authorize(Policy = "DoctorOnly")]
        public async Task<IActionResult> update(int id,[FromForm] PatientModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);



            Patient? patient = await _context.Patients.FindAsync(id);

            if(patient == null)
            {
                return NotFound();
            }

            patient.Name = model.Name;
            patient.Height = model.Height;
            patient.Weight = model.Weight;
            patient.Nic = model.Nic;
            patient.RegisteredBy = 1;
            patient.Gender = model.Gender switch
            {
                "Female" => Gender.Female,
                _=> Gender.Male
            };
            patient.BloodGroup = model.BloodGroup switch
            {
                "A" => BloodGroup.A,
                "B" => BloodGroup.B,
                _=> BloodGroup.O
            };
            patient.Address = model.Address;

            //Save the profile image
            /*if(model.profileImage != null && model.profileImage.Length > 0)
            {
                var fileExtension = Path.GetExtension(model.profileImage.FileName);
                var filePath = $"/uploads/profile/patient-{id}{fileExtension}";

                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.profileImage.CopyToAsync(stream);
                }
            }*/

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Type = "Success",
                Message = "Patient updated successfully"
            });
        }

        private int GetRelevanceMark(string searchInput, string searchQuery)
        {
            if (!searchInput.Contains(searchQuery)) return 0;

            int numOfCharsBefore = searchInput.IndexOf(searchQuery);
            return numOfCharsBefore;
        }
    }

}

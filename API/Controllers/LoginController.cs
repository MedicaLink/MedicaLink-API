using API.Data;
using API.Models;
using API.Models.FormModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[Controller]")]
    public class LoginController : ControllerBase
    {
        ApplicationDbContext _context;
        private readonly IPasswordHasher<Admin> _adminPasswordHasher;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Patient> _patientPasswordHasher;

        public LoginController(ApplicationDbContext applicationDbContext, IConfiguration configuration, IPasswordHasher<Admin> adminPasswordHasher, IPasswordHasher<Patient> patientPasswordHasher)
        {
            _context = applicationDbContext;
            _configuration = configuration;
            _adminPasswordHasher = adminPasswordHasher;
            _patientPasswordHasher = patientPasswordHasher;
        }

        [HttpPost]
        public async Task<IActionResult> HandleLogin([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Authenticate the user and send the JWT
            var admin = await _context.Admins.SingleOrDefaultAsync(a => a.Email == loginModel.UserName);
            var patient = await _context.Patients.SingleOrDefaultAsync(p => p.Nic == loginModel.UserName);

            if (admin == null && patient == null) 
            {
                return Unauthorized();
            }

            List<string> userRoles = [];
            userRoles.Add(admin != null ? "Doctor" : "User");

            var userRole = userRoles[0];
            int userId; string? name;

            if (admin != null) 
            {
                userId = admin.Id;
                name = admin.Name;
            }
            else
            {
                userId = patient.Id;
                name = patient.Name;
            }
            

            if (userRoles[0] == "Doctor" && admin.Type == AdminType.SuperAdmin) // If the user is an Admin check if he/she is a doctor
            {
                userRoles.Add("Admin");
                userRole = "Admin";
            }

            var JWTToken = GenerateJWT(loginModel.UserName, userRoles, userId); // Generate the JWT

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(JWTToken),
                expiration = JWTToken.ValidTo,
                userId,
                name,
                userName = loginModel.UserName,
                role = userRole,
                allRoles = userRoles
            });
        }

        private JwtSecurityToken GenerateJWT(string Username, List<string> UserRoles, int UserId)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.PrimarySid, UserId.ToString()),
            };

            UserRoles.ForEach(role =>
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            });

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}

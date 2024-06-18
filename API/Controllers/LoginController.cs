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

            var userRole = admin != null ? "Admin" : "Patient";
            var userId = admin != null ? admin.Id : patient.Id;

            var JWTToken = GenerateJWT(loginModel.UserName, userRole, userId); // Generate the JWT

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(JWTToken),
                expiration = JWTToken.ValidTo
            });
        }

        private JwtSecurityToken GenerateJWT(string Username, string UserRole, int UserId)
        {
            var authClaims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
            new Claim(ClaimTypes.Role, UserRole)
        };

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

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Projekt_ISS_be.Data;
using Projekt_ISS_be.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_ISS_be.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const string SecretKey = "h4$J8ks!29LmQ#fD92aPqZxT7wB*eR1D";

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Firstname) ||
                    string.IsNullOrWhiteSpace(request.Lastname) ||
                    string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Vsa polja so obvezna" });
                }

                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower()))
                {
                    return BadRequest(new { message = "Email je že registriran" });
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var newUser = new User
                {
                    FirstName = request.Firstname,
                    LastName = request.Lastname,
                    Email = request.Email.ToLower(),
                    Password = hashedPassword,
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Registracija uspešna",
                    user = new
                    {
                        id = newUser.UserId,
                        email = newUser.Email,
                        firstname = newUser.FirstName,
                        lastname = newUser.LastName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Napaka pri registraciji", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Email in geslo sta obvezna" });
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    return Unauthorized(new { message = "Neveljavni podatki" });
                }

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    token = token,
                    user = new
                    {
                        id = user.UserId,
                        email = user.Email,
                        firstname = user.FirstName,
                        lastname = user.LastName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Napaka pri prijavi", error = ex.Message });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
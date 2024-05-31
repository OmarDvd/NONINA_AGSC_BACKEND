using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NONINA_AGSC_APP_BACKEND.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using NONINA_AGSC_APP_BACKEND_API.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Azure.Identity;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticationController : ControllerBase
    {
        private readonly string secretKey;
        private readonly UserAPIDbContext _dbContext;
        public AutenticationController(IConfiguration config, UserAPIDbContext dbContext)
        {
            secretKey = config.GetSection("settings").GetSection("secretKey").ToString();
            _dbContext = dbContext;
        }



        [HttpPost]
        [Route("validar")]
        public async Task<IActionResult> Validar([FromBody] Usuario request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.username);

            if (user == null)
            {
                return NotFound("El recurso solicitado no fue encontrado. Por favor, revise los parámetros proporcionados.");
            }
            var claveHasSalt = user.Salt;
            string hashedPassword = HashPassword(request.clave, claveHasSalt);
            if (hashedPassword != user.Password)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new {id=user.Id,admin=user.Admin,owner=user.Owner,name=user.Name,email=user.Email, token = "" });
            }
            
                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();

                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.username));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
                string tokencreado = tokenHandler.WriteToken(tokenConfig);
                return StatusCode(StatusCodes.Status200OK, new { token = tokencreado });
            

        }

        // Método para hashear la contraseña junto con la sal
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}

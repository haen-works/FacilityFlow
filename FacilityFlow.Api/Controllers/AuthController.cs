using FacilityFlow.Api.Data;
using FacilityFlow.Api.Dtos;
using FacilityFlow.Api.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FacilityFlow.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(
            AppDbContext context,
            IPasswordHasher<AppUser> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // Kullanıcının yazdığı emaili standart hale getiriyoruz.
            var email = dto.Email.Trim().ToLowerInvariant();

            // Email ile kullanıcıyı veritabanında arıyoruz.
            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Email == email);

            // Kullanıcı yoksa giriş başarısız.
            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "E-posta veya şifre hatalı."
                });
            }

            // Girilen şifreyi veritabanındaki hash ile karşılaştırıyoruz.
            var passwordResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password);

            // Şifre yanlışsa giriş başarısız.
            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "E-posta veya şifre hatalı."
                });
            }

            // Token 2 saat geçerli olacak.
            var expiresAt = DateTime.UtcNow.AddHours(2);

            // JWT token oluşturuyoruz.
            var token = CreateToken(user, expiresAt);

            // Kullanıcıya token ve temel bilgileri gönderiyoruz.
            return Ok(new
            {
                token,
                expiresAt,

                user = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Role
                }
            });
        }

        private string CreateToken(AppUser user, DateTime expiresAt)
        {
            // Token içerisine kullanıcının kimlik bilgilerini koyuyoruz.
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.FullName),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            // appsettings.json içerisindeki gizli JWT anahtarını alıyoruz.
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT anahtarı bulunamadı.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            // Tokenı imzalamak için kullanılacak algoritma.
            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            // JWT token oluşturuluyor.
            var jwtToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            // Tokenı uzun bir string haline getirip döndürüyoruz.
            return new JwtSecurityTokenHandler()
                .WriteToken(jwtToken);
        }
    }
}
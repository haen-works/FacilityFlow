using FacilityFlow.Api.Data;
using FacilityFlow.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FacilityFlow.Api.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FacilityFlow.Api.Controllers


{
    [ApiController]
    [Route("api/users")]

    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public UsersController(AppDbContext context, IPasswordHasher<AppUser>passwordHasher) 
        {
            _context = context;
            _passwordHasher= passwordHasher;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> Create(CreateUserDto dto, [FromServices] IWebHostEnvironment environment)
        {
            if(!environment.IsDevelopment())
            {
                return NotFound();
            }

            var email = dto.Email.Trim().ToLowerInvariant();

            var emailExists = await _context.Users
                .AnyAsync(user => user.Email == email);

            if(emailExists)
            {
                return Conflict(new
                {
                    message = "Bu e-posta adresi zaten kullanılıyor."
                });
            }

            var user = new AppUser
            {
                FullName = dto.FullName.Trim(),
                Email = email,
                Role = dto.Role
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                dto.Password);


            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };

            return StatusCode(201, response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserResponseDto>>> Get()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Select(user=>new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}
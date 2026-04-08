using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentFlowApi.DTO;
using RentFlowApi.Model;
using RentFlowApi.Services;
using System;
using System.Net.WebSockets;
using System.Security.Claims;

namespace RentFlowApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RentflowContext _context;
        private readonly JwtService _jwtService;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthController(RentflowContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<User>();
        }

        

        [HttpPost("registerUser")]
        public async Task<IActionResult> RegisterUser(RegisterUserDto dto)
        {

            // если я регистрирую пользователя то я должен вписать ему роль что он пользователь (
            // как это сделать передавать сущность или не ебаться передать id ??)
            // если я создаю менеджера мне нужно передат ему роль и id хозяина
            // если я создаю хозяина то мне нужно созадть сущность Owner потом создать пользователя дать ему роль
            // хозяина и передать в поле номер компании.

            var existingUser = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Phone == dto.Phone);

            if (existingUser != null)
                return BadRequest(new { message = "Пользователь уже существует" });

            var user = new User
            {
                Phone = dto.Phone,
                RoleId = 3,
                Role = await _context.Roles.FirstAsync(u => u.Id == 3),
                FirstName = dto.FirstName,
                IsActive = true,
                CreatedAt = DateTime.Now,
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            var client_profile = new ClientProfile
            {
                UserId = user.Id,
                LeadSourceId = dto.LeadSourceId,
            };

            _context.ClientProfiles.Add(client_profile);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token
            });


        }

        [HttpPost("registerManager")]
        public async Task<ActionResult> RegisterManager(RegManagerDTO dto)
        {
            var existingUser = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Phone == dto.Phone);

            if (existingUser != null)
                return BadRequest(new { message = "Пользователь уже существует" });

            var user = new User
            {
                Phone = dto.Phone,
                RoleId = 2,
                Role = await _context.Roles.FirstAsync(u => u.Id == 2),
                OwnerId = dto.OwnerId,
                FirstName = dto.FirstName,
                IsActive = true,
                CreatedAt = DateTime.Now,

            };


            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token
            });

        }

        [HttpPost("registerOwner")]
        public async Task<ActionResult> RegisterOwner(RegisterOwnerDto dto)
        {
            var existingUser = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Phone == dto.Phone);

            if (existingUser != null)
                return BadRequest(new { message = "Пользователь уже существует" });

            var owner = new Owner
            {
                CompanyName = dto.CompanyName,
                Phone = dto.Phone,
                Telegram = dto.Telegram,                
                Email = dto.Email,
                Description = dto.Description,

            };

            _context.Owners.Add(owner);
            owner.PublicSlug = await CreateUniqueSlugAsync();
            await _context.SaveChangesAsync();

            
           

            var user = new User
            {
                Phone = dto.Phone,
                RoleId = 1, 
                Role = await _context.Roles.FirstAsync(u=> u.Id == 1),
                OwnerId = owner.Id,
                FirstName = dto.FirstName,
                IsActive = true,
                CreatedAt = DateTime.Now,
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token
            });

        }

        private async Task<string> CreateUniqueSlugAsync()
        {
            string slug;

            do
            {
                slug = Guid.NewGuid().ToString("N").Substring(0, 10);
            }
            while (await _context.Owners.AnyAsync(x => x.PublicSlug == slug));

            return slug;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetPErInfo()
        {
            string? cl = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;
            if (cl == null) return BadRequest();
            return Ok(cl);
            
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult> Me()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                id = user.Id,
                
                ownerId = user.OwnerId,
                roleId = user.RoleId
            });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Phone == dto.Phone);

            if (user == null)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Неверный логин или пароль" });

            var token = _jwtService.GenerateToken(user);


            return Ok(new
            {
                token,
                userId = user.Id,
                phone = user.Phone,
                roleId = user.RoleId,
                fullName = user.FullName
            });
        }

        
    }
}

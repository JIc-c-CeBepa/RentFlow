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
using System.Security.Cryptography;

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

        private string GenerateRefreshTokenValue()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        private async Task<RefreshToken> CreateRefreshTokenAsync(int userId)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = GenerateRefreshTokenValue(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private static bool IsRefreshTokenActive(RefreshToken refreshToken)
        {
            return !refreshToken.IsRevoked && refreshToken.ExpiresAt > DateTime.UtcNow;
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Refresh token обязателен" });

            var existingRefreshToken = await _context.RefreshTokens
                .Include(x => x.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

            if (existingRefreshToken == null)
                return Unauthorized(new { message = "Refresh token не найден" });

            if (!IsRefreshTokenActive(existingRefreshToken))
                return Unauthorized(new { message = "Refresh token недействителен или истек" });

            var user = existingRefreshToken.User;
            if (user == null)
                return Unauthorized(new { message = "Пользователь не найден" });

            if (!user.IsActive.Value)
                return Unauthorized(new { message = "Пользователь деактивирован" });

            existingRefreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            var newAccessToken = _jwtService.GenerateToken(user);
            var newRefreshToken = await CreateRefreshTokenAsync(user.Id);

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken.Token
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Refresh token обязателен" });

            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

            if (refreshToken == null)
                return Ok(new { message = "Выход выполнен" });

            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Выход выполнен" });
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<ActionResult> LogoutAll()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Не удалось определить пользователя" });

            var userRefreshTokens = await _context.RefreshTokens
                .Where(x => x.UserId == userId && !x.IsRevoked)
                .ToListAsync();

            foreach (var token in userRefreshTokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Выход со всех устройств выполнен" });
        }

        [HttpPost("registerUser")]
        public async Task<IActionResult> RegisterUser(RegisterUserDto dto)
        {
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
            var refreshToken = await CreateRefreshTokenAsync(user.Id);

            return Ok(new
            {
                accessToken = token,
                refreshToken = refreshToken.Token
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
            var refreshToken = await CreateRefreshTokenAsync(user.Id);

            return Ok(new
            {
                accessToken = token,
                refreshToken = refreshToken.Token
            });

        }

        [Authorize]
        [HttpPost("register-company")]
        public async Task<ActionResult> RegisterCompanyFromCurrentUser([FromBody] RegisterOwnerDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Не удалось определить пользователя" });

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            if (user.OwnerId != null)
                return BadRequest(new { message = "Пользователь уже привязан к компании" });

            if (string.IsNullOrWhiteSpace(dto.CompanyName))
                return BadRequest(new { message = "Название компании обязательно" });

            var companyName = dto.CompanyName.Trim();
            var description = dto.Description?.Trim();
            var firstName = dto.FirstName?.Trim();
            var lastName = dto.LastName?.Trim();
            var middleName = dto.MiddleName?.Trim();
            var telegram = dto.Telegram?.Trim();
            

            var owner = new Owner
            {
                CompanyName = companyName,
                Phone = user.Phone,               
                Telegram = telegram,
                Description = string.IsNullOrWhiteSpace(description) ? null : description,

                
            };

            _context.Owners.Add(owner);
            await _context.SaveChangesAsync();

            owner.PublicSlug = await CreateUniqueSlugAsync();
            await _context.SaveChangesAsync();

            
            if (string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(firstName))
                user.FirstName = firstName;

            if (string.IsNullOrWhiteSpace(user.LastName) && !string.IsNullOrWhiteSpace(lastName))
                user.LastName = lastName;

            if (string.IsNullOrWhiteSpace(user.MiddleName) && !string.IsNullOrWhiteSpace(middleName))
                user.MiddleName = middleName;

            if (string.IsNullOrWhiteSpace(user.Telegram) && !string.IsNullOrWhiteSpace(telegram))
                user.Telegram = telegram;

            user.OwnerId = owner.Id;
            user.RoleId = 1;

            await _context.SaveChangesAsync();

            var updatedUser = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == user.Id);

            if (updatedUser == null)
                return NotFound(new { message = "Пользователь не найден после обновления" });

            var newToken = _jwtService.GenerateToken(updatedUser);

            var refreshToken = await CreateRefreshTokenAsync(updatedUser.Id);

            return Ok(new
            {
                message = "Компания успешно зарегистрирована",
                accessToken = newToken,
                refreshToken = refreshToken.Token,
                owner = new
                {
                    id = owner.Id,
                    companyName = owner.CompanyName,
                    phone = owner.Phone,
                    telegram = owner.Telegram,
                    publicSlug = owner.PublicSlug,
                    description = owner.Description
                }
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

            string? photoBase64 = null;

            if (user.Photo != null && user.Photo.Length > 0)
            {
                photoBase64 = $"data:image/jpeg;base64,{Convert.ToBase64String(user.Photo)}";
            }

            return Ok(new
            {
                id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                middleName = user.MiddleName,
                phone = user.Phone,
                telegram = user.Telegram,
                photo = photoBase64,
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
            var refreshToken = await CreateRefreshTokenAsync(user.Id);

            return Ok(new
            {
                accessToken = token,
                refreshToken = refreshToken.Token,
                userId = user.Id,
                phone = user.Phone,
                roleId = user.RoleId,
                fullName = user.FullName
            });
        }

        [Authorize]
        [HttpPost("upload-avatar")]
        public async Task<ActionResult> UploadAvatar([FromForm] UploadAvatarRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Не удалось определить пользователя" });

            if (request.Avatar == null || request.Avatar.Length == 0)
                return BadRequest(new { message = "Файл не выбран" });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(request.Avatar.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Допустимы только jpg, jpeg, png, webp" });

            if (request.Avatar.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "Максимальный размер файла 5 МБ" });

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            using var memoryStream = new MemoryStream();
            await request.Avatar.CopyToAsync(memoryStream);
            user.Photo = memoryStream.ToArray();

            await _context.SaveChangesAsync();

            var photoBase64 = $"data:image/{GetImageFormat(extension)};base64,{Convert.ToBase64String(user.Photo)}";

            return Ok(new
            {
                message = "Аватар успешно обновлен",
                photo = photoBase64
            });
        }

        private string GetImageFormat(string extension)
        {
            return extension switch
            {
                ".jpg" => "jpeg",
                ".jpeg" => "jpeg",
                ".png" => "png",
                ".webp" => "webp",
                _ => "jpeg"
            };
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateClientProfileRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized(new { message = "Не удалось определить пользователя" });

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });

            var firstName = request.FirstName?.Trim();
            var lastName = request.LastName?.Trim();
            var middleName = request.MiddleName?.Trim();
            var telegram = request.Telegram?.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
                return BadRequest(new { message = "Имя обязательно" });

            if (firstName.Length > 100)
                return BadRequest(new { message = "Имя слишком длинное" });

            if (!string.IsNullOrEmpty(lastName) && lastName.Length > 100)
                return BadRequest(new { message = "Фамилия слишком длинная" });

            if (!string.IsNullOrEmpty(middleName) && middleName.Length > 100)
                return BadRequest(new { message = "Отчество слишком длинное" });

            if (!string.IsNullOrEmpty(telegram) && telegram.Length > 100)
                return BadRequest(new { message = "Telegram слишком длинный" });

            user.FirstName = firstName;
            user.LastName = string.IsNullOrWhiteSpace(lastName) ? null : lastName;
            user.MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName;
            user.Telegram = string.IsNullOrWhiteSpace(telegram) ? null : telegram;

            await _context.SaveChangesAsync();

            string? photoBase64 = null;
            if (user.Photo != null && user.Photo.Length > 0)
            {
                photoBase64 = $"data:image/jpeg;base64,{Convert.ToBase64String(user.Photo)}";
            }

            return Ok(new
            {
                message = "Профиль обновлен",
                user = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    middleName = user.MiddleName,
                    phone = user.Phone,
                    telegram = user.Telegram,
                    photo = photoBase64,
                    ownerId = user.OwnerId,
                    roleId = user.RoleId
                }
            });
        }


    }
}

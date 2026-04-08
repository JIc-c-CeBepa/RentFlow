using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentFlowApi.DTO;
using RentFlowApi.Model;
using RentFlowApi.Services;
using System.Security.Claims;

namespace RentFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly RentflowContext _context;
        private readonly IFileStorageInterface _fileStorageService;

        public PropertyController(RentflowContext context, IFileStorageInterface fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        

        [Authorize]
        [HttpPost("{propertyId}/images")]
        public async Task<ActionResult> UploadPropertyImage(int propertyId, [FromForm] UploadPropertyImageRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Файл не выбран");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Не удалось определить пользователя");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return NotFound("Пользователь не найден");

            if (user.OwnerId == null)
                return BadRequest("Пользователь не привязан к владельцу");

            var property = await _context.Properties
                .FirstOrDefaultAsync(x => x.Id == propertyId && x.OwnerId == user.OwnerId);

            if (property == null)
                return NotFound("Квартира не найдена или нет доступа");

            string relativePath;
            try
            {
                relativePath = await _fileStorageService.SavePropertyImageAsync(request.File);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            if (request.IsMain)
            {
                var oldMainImages = await _context.PropertyPhotos
                    .Where(x => x.PropertyId == propertyId && x.IsMain)
                    .ToListAsync();

                foreach (var img in oldMainImages)
                    img.IsMain = false;
            }

            var propertyImage = new PropertyPhoto
            {
                PropertyId = propertyId,
                PublicUrl = relativePath,
                IsMain = request.IsMain,
                SortOrder = request.SortOrder,
                CreatedAt = DateTime.UtcNow
            };

            _context.PropertyPhotos.Add(propertyImage);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Фото загружено",
                imageId = propertyImage.Id,
                imageUrl = $"{Request.Scheme}://{Request.Host}{propertyImage.PublicUrl}"
            });
        }

        [Authorize]
        [HttpGet("GetPropertyByOwner")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetPropertyByOwner()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Не удалось определить пользователя");

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return NotFound("Пользователь не найден");

            if (user.OwnerId == null)
                return BadRequest("Пользователь не привязан к владельцу");

            var properties = await _context.Properties
                .AsNoTracking()
                .Include(x => x.PropertyPhotos)
                .Where(x => x.OwnerId == user.OwnerId)
                .ToListAsync();

            var result = properties.Select(x => new PropertyDto
            {
                Id = x.Id,
                Title = x.Title,
                Address = x.Address,
                MainImageUrl = x.PropertyPhotos
                    .Where(i => i.IsMain)
                    .OrderBy(i => i.SortOrder)
                    .Select(i => $"{Request.Scheme}://{Request.Host}{i.PublicUrl}")
                    .FirstOrDefault(),
                Images = x.PropertyPhotos
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => new PropertyImageDto
                    {
                        Id = i.Id,
                        ImageUrl = $"{Request.Scheme}://{Request.Host}{i.PublicUrl}",
                        IsMain = i.IsMain,
                        SortOrder = i.SortOrder
                    })
                    .ToList()
            }).ToList();

            return Ok(result);
        }
    }
}

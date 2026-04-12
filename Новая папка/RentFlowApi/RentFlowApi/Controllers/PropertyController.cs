using Microsoft.AspNetCore.Authorization;
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
                Description = x.Description,
                CurrentPrice = x.CurrentPrice,
                MaxGuests = x.MaxGuests,
                IsContactlessCheckInAvailable = x.IsContactlessCheckinAvailable,
                BookingMode = x.BookingMode,
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

        [Authorize]
        [HttpGet("my-properties")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetPropertiesForCurrentUser()
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
                Description = x.Description,
                CurrentPrice = x.CurrentPrice,
                MaxGuests = x.MaxGuests,
                IsContactlessCheckInAvailable = x.IsContactlessCheckinAvailable,
                BookingMode = x.BookingMode,
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

        [Authorize]
        [HttpGet("my-properties/filter")]
        public async Task<ActionResult<IEnumerable<PropertyCatalogItemDto>>> FilterCatalog([FromQuery] PropertyCatalogFilterRequest request)
        {
            if (request.MinPrice.HasValue && request.MaxPrice.HasValue && request.MinPrice > request.MaxPrice)
                return BadRequest("Минимальная цена не может быть больше максимальной");

            if (request.GuestsCount.HasValue && request.GuestsCount <= 0)
                return BadRequest("Количество гостей должно быть больше 0");

            if (request.ArrivalDate.HasValue && request.DepartureDate.HasValue)
            {
                if (request.ArrivalDate >= request.DepartureDate)
                    return BadRequest("Дата выезда должна быть позже даты заезда");
            }
            else if (request.ArrivalDate.HasValue || request.DepartureDate.HasValue)
            {
                return BadRequest("Для проверки занятости нужно передать и дату заезда, и дату выезда");
            }

            var query = _context.Properties
                .AsNoTracking()
                .Include(x => x.PropertyPhotos)
                .Include(x => x.Owner)
                .Where(x => x.IsActive == true);

            if (!string.IsNullOrWhiteSpace(request.OwnerSlug))
            {
                query = query.Where(x => x.Owner.PublicSlug == request.OwnerSlug);
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(x => x.CurrentPrice >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(x => x.CurrentPrice <= request.MaxPrice.Value);
            }

            if (request.GuestsCount.HasValue)
            {
                query = query.Where(x => x.MaxGuests >= request.GuestsCount.Value);
            }

            if (request.NeedContactlessCheckIn.HasValue && request.NeedContactlessCheckIn.Value)
            {
                query = query.Where(x => x.IsContactlessCheckinAvailable);
            }

            if (request.ArrivalDate.HasValue && request.DepartureDate.HasValue)
            {
                query = query.Where(p => !_context.Bookings.Any(b =>
                    b.PropertyId == p.Id &&
                    b.Status.Name != "Отменено" &&
                    b.ArrivalDate < request.DepartureDate.Value &&
                    b.DepartureDate > request.ArrivalDate.Value
                ));
            }

            var properties = await query
                .OrderBy(x => x.CurrentPrice)
                .ToListAsync();

            var result = properties.Select(x => new PropertyCatalogItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Address = x.Address,
                Description = x.Description,
                CurrentPrice = x.CurrentPrice,
                MaxGuests = x.MaxGuests,
                IsContactlessCheckInAvailable = x.IsContactlessCheckinAvailable,
                BookingMode = x.BookingMode,
                MainImageUrl = x.PropertyPhotos
                    .Where(i => i.IsMain)
                    .OrderBy(i => i.SortOrder)
                    .Select(i => $"{Request.Scheme}://{Request.Host}{i.PublicUrl}")
                    .FirstOrDefault()
                    ?? x.PropertyPhotos
                        .OrderBy(i => i.SortOrder)
                        .Select(i => $"{Request.Scheme}://{Request.Host}{i.PublicUrl}")
                        .FirstOrDefault()
            }).ToList();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{propertyId}")]
        public async Task<ActionResult<PropertyDetailsDto>> GetPropertyById(int propertyId)
        {
            var property = await _context.Properties
                .AsNoTracking()
                .Include(x => x.PropertyPhotos)
                .Include(x => x.Amenities)
                .Include(x => x.Rules)
                .FirstOrDefaultAsync(x => x.Id == propertyId && x.IsActive == true);

            if (property == null)
                return NotFound("Квартира не найдена");

            var result = new PropertyDetailsDto
            {
                Id = property.Id,
                Title = property.Title,
                Address = property.Address,
                Description = property.Description,
                CurrentPrice = property.CurrentPrice,
                MaxGuests = property.MaxGuests,
                IsContactlessCheckInAvailable = property.IsContactlessCheckinAvailable,
                BookingMode = property.BookingMode,
                MainImageUrl = property.PropertyPhotos
                    .Where(i => i.IsMain)
                    .OrderBy(i => i.SortOrder)
                    .Select(i => $"{Request.Scheme}://{Request.Host}{i.PublicUrl}")
                    .FirstOrDefault()
                    ?? property.PropertyPhotos
                        .OrderBy(i => i.SortOrder)
                        .Select(i => $"{Request.Scheme}://{Request.Host}{i.PublicUrl}")
                        .FirstOrDefault(),
                Images = property.PropertyPhotos
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => new PropertyImageDto
                    {
                        Id = i.Id,
                        ImageUrl = $"{Request.Scheme}://{Request.Host}{i.PublicUrl}",
                        IsMain = i.IsMain,
                        SortOrder = i.SortOrder
                    })
                    .ToList(),
                Amenities = property.Amenities
                    .Where(x => x.IsActive.Value)
                    .Select(x => new AmenityDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Icon = x.Icon
                    })
                    .ToList(),
                Rules = property.Rules
                    .Where(x => x.IsActive.Value)
                    .Select(x => new RuleDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description
                    })
                    .ToList()
            };

            return Ok(result);
        
    }
    }
}
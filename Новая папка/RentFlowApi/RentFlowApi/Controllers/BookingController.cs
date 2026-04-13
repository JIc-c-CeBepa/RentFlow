using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentFlowApi.DTO;
using RentFlowApi.Model;
using System.Security.Claims;

namespace RentFlowApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly RentflowContext _context;

        public BookingController(RentflowContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BookingCreatedDto>> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Не удалось определить пользователя");

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return NotFound("Пользователь не найден");

            if (user.RoleId != 3)
                return Forbid("Бронировать может только клиент");

            if (request.GuestsCount <= 0)
                return BadRequest("Количество гостей должно быть больше 0");

            if (request.ArrivalDate >= request.DepartureDate)
                return BadRequest("Дата выезда должна быть позже даты заезда");

            var property = await _context.Properties
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.PropertyId && x.IsActive == true);

            if (property == null)
                return NotFound("Квартира не найдена");

            if (request.GuestsCount > property.MaxGuests)
                return BadRequest($"Максимальное количество гостей: {property.MaxGuests}");

            if (request.NeedsContactlessCheckin && !property.IsContactlessCheckinAvailable)
                return BadRequest("Для этой квартиры бесконтактное заселение недоступно");

            var canceledStatusId = await _context.BookingStatuses
                .Where(x => x.Name == "Отменено")
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var hasOverlap = await _context.Bookings.AnyAsync(b =>
                b.PropertyId == request.PropertyId &&
                b.StatusId != canceledStatusId &&
                b.ArrivalDate < request.DepartureDate &&
                b.DepartureDate > request.ArrivalDate);

            if (hasOverlap)
                return BadRequest("Квартира уже занята на выбранные даты");

            var nightsCount = request.DepartureDate.DayNumber - request.ArrivalDate.DayNumber;
            if (nightsCount <= 0)
                return BadRequest("Некорректный диапазон дат");

            var statusName = property.BookingMode == "instant"
                ? "Подтверждено"
                : "Ожидает подтверждения";

            var statusId = await _context.BookingStatuses
                .Where(x => x.Name == statusName)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (statusId == 0)
                return BadRequest($"Не найден статус бронирования: {statusName}");

            var pricePerNight = property.CurrentPrice;
            var totalAmount = pricePerNight * nightsCount;
            var prepaymentPercent = property.PrepaymentPercent;
            var prepaymentAmount = Math.Round(totalAmount * prepaymentPercent / 100m, 2);

            var booking = new Booking
            {
                PropertyId = property.Id,
                UserId = userId,
                StatusId = statusId,
                ArrivalDate = request.ArrivalDate,
                DepartureDate = request.DepartureDate,
                GuestsCount = request.GuestsCount,
                NeedsContactlessCheckin = request.NeedsContactlessCheckin,
                PricePerStay = totalAmount,
                PrepaymentPercent = prepaymentPercent,
                PrepaymentAmount = prepaymentAmount,
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow,
                ConfirmedAt = property.BookingMode == "instant" ? DateTime.UtcNow : null
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var result = new BookingCreatedDto
            {
                Id = booking.Id,
                PropertyId = property.Id,
                PropertyTitle = property.Title,
                ArrivalDate = booking.ArrivalDate,
                DepartureDate = booking.DepartureDate,
                NightsCount = nightsCount,
                GuestsCount = booking.GuestsCount,
                Status = statusName,
                PricePerNight = pricePerNight,
                TotalAmount = booking.TotalAmount,
                PrepaymentPercent = booking.PrepaymentPercent,
                PrepaymentAmount = booking.PrepaymentAmount,
                NeedsContactlessCheckin = booking.NeedsContactlessCheckin,
                CreatedAt = booking.CreatedAt,
                StatusId = statusId
            };

            return Ok(result);
        }

        [Authorize]
        [HttpGet("my-bookings")]
        public async Task<ActionResult> GetMyBookings()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Не удалось определить пользователя");

            var bookings = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Property)
                    .ThenInclude(p => p.Owner)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new
                {
                    b.Id,
                    b.ArrivalDate,
                    b.DepartureDate,
                    b.GuestsCount,
                    b.TotalAmount,
                    b.PrepaymentAmount,
                    b.PrepaymentPercent,
                    b.StatusId,
                    b.CreatedAt,
                    b.NeedsContactlessCheckin,
                    Property = new
                    {
                        b.Property.Title,
                        b.Property.Address,
                        b.Property.MaxGuests,
                        b.Property.CheckInTime,
                        b.Property.CheckOutTime,
                        Owner = b.Property.Owner != null ? new
                        {
                            b.Property.Owner.CompanyName,
                            b.Property.Owner.Phone,
                            b.Property.Owner.Email,
                            b.Property.Owner.Telegram
                        } : null
                    }
                })
                .ToListAsync();

            return Ok(bookings);
        }
    }
}

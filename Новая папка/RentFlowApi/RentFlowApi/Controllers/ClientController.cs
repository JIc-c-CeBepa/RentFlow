using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class ClientController : ControllerBase
    {
        private readonly RentflowContext _context;

        public ClientController(RentflowContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("AddOwnerLinkToUser")]
        public async Task<ActionResult> AddOwnerLinkToUser(AddOwnerLinkRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.code))
                return BadRequest("Код пустой"); 

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Не удалось определить пользователя");

            Owner owner = await _context.Owners.FirstOrDefaultAsync(o=> o.PublicSlug == request.code);

            if (owner == null)
                return NotFound("Неверная ссылка");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("Пользователь не найден");

            user.OwnerId = owner.Id;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Привязка выполнена" });

        }
    }
}

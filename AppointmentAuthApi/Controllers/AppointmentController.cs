using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppointmentAuthApi.DTOs;
using AppointmentAuthApi.Services;

namespace AppointmentAuthApi.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpGet("slots")]
        public async Task<IActionResult> GetSlots()
            => Ok(await _service.GetAllSlotsAsync());

        [HttpPost("book")]
        public async Task<IActionResult> Book(BookSlotDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _service.BookSlotAsync(userId, dto.SlotId);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyBookings()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            return Ok(await _service.GetUserAppointmentsAsync(userId));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _service.CancelAppointmentAsync(id, userId);
            return Ok(result);
        }
    }
}

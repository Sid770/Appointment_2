using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentAuthApi.Data;
using AppointmentAuthApi.Models;

namespace AppointmentAuthApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class SlotController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SlotController> _logger;

        public SlotController(AppDbContext context, ILogger<SlotController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Create a test slot (Admin endpoint - no auth required for testing)
        /// </summary>
        [HttpPost("slots/create")]
        public async Task<IActionResult> CreateSlot([FromBody] CreateSlotRequest request)
        {
            try
            {
                if (request.StartTime >= request.EndTime)
                    return BadRequest(new { message = "StartTime must be before EndTime" });

                var slot = new Slot
                {
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    CreatedBy = request.CreatedBy ?? "Admin"
                };

                _context.Slots.Add(slot);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Slot created: {slot.SlotID} from {slot.StartTime} to {slot.EndTime}");

                return Ok(new
                {
                    success = true,
                    message = "Slot created successfully",
                    data = new
                    {
                        slotID = slot.SlotID,
                        startTime = slot.StartTime,
                        endTime = slot.EndTime
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating slot: {ex.Message}");
                return StatusCode(500, new { message = "Error creating slot", error = ex.Message });
            }
        }

        /// <summary>
        /// Get all slots (Admin endpoint)
        /// </summary>
        [HttpGet("slots")]
        public async Task<IActionResult> GetAllSlots()
        {
            try
            {
                var slots = await _context.Slots.ToListAsync();
                return Ok(new
                {
                    success = true,
                    message = $"Retrieved {slots.Count} slots",
                    data = slots.Select(s => new
                    {
                        s.SlotID,
                        s.StartTime,
                        s.EndTime,
                        s.CreatedBy,
                        IsBooked = s.Appointment != null
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving slots", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a slot (Admin endpoint)
        /// </summary>
        [HttpDelete("slots/{id}")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            try
            {
                var slot = await _context.Slots.FindAsync(id);
                if (slot == null)
                    return NotFound(new { message = "Slot not found" });

                _context.Slots.Remove(slot);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Slot deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting slot", error = ex.Message });
            }
        }
    }

    public class CreateSlotRequest
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? CreatedBy { get; set; }
    }
}

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
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(IAppointmentService service, ILogger<AppointmentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get all available slots (requires JWT authentication)
        /// </summary>
        /// <returns>List of all available appointment slots</returns>
        [HttpGet("slots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<SlotDto>>>> GetSlots()
        {
            try
            {
                var result = await _service.GetAllSlotsAsync();
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching slots: {ex.Message}");
                return StatusCode(500, new ApiResponse<List<SlotDto>>
                {
                    Success = false,
                    Message = "Error fetching slots",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Book an appointment slot (requires JWT authentication)
        /// </summary>
        /// <param name="dto">Slot ID to book</param>
        /// <returns>Booking confirmation with appointment details</returns>
        [HttpPost("book")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<AppointmentResultDto>>> Book(BookSlotDto dto)
        {
            try
            {
                if (dto.SlotId <= 0)
                    return BadRequest(new ApiResponse<AppointmentResultDto>
                    {
                        Success = false,
                        Message = "Invalid slot ID",
                        Error = "SlotId must be greater than 0"
                    });

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new ApiResponse<AppointmentResultDto>
                    {
                        Success = false,
                        Message = "Invalid user information",
                        Error = "Could not extract user ID from token"
                    });

                _logger.LogInformation($"User {userId} attempting to book slot {dto.SlotId}");
                var result = await _service.BookSlotAsync(userId, dto.SlotId);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error booking appointment: {ex.Message}");
                return StatusCode(500, new ApiResponse<AppointmentResultDto>
                {
                    Success = false,
                    Message = "Error booking appointment",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get user's appointments (requires JWT authentication)
        /// </summary>
        /// <returns>List of user's booked appointments</returns>
        [HttpGet("my")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> MyBookings()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new ApiResponse<List<AppointmentDto>>
                    {
                        Success = false,
                        Message = "Invalid user information",
                        Error = "Could not extract user ID from token"
                    });

                _logger.LogInformation($"User {userId} fetching their bookings");
                var result = await _service.GetUserAppointmentsAsync(userId);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching user appointments: {ex.Message}");
                return StatusCode(500, new ApiResponse<List<AppointmentDto>>
                {
                    Success = false,
                    Message = "Error fetching appointments",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Cancel an appointment (requires JWT authentication)
        /// </summary>
        /// <param name="id">Appointment ID to cancel</param>
        /// <returns>Cancellation result</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> Cancel(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid appointment ID",
                        Error = "AppointmentId must be greater than 0"
                    });

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid user information",
                        Error = "Could not extract user ID from token"
                    });

                _logger.LogInformation($"User {userId} cancelling appointment {id}");
                var result = await _service.CancelAppointmentAsync(id, userId);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error cancelling appointment: {ex.Message}");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Error cancelling appointment",
                    Error = ex.Message
                });
            }
        }
    }
}

using AppointmentAuthApi.Data;
using AppointmentAuthApi.DTOs;
using Microsoft.EntityFrameworkCore;
using AppointmentAuthApi.Models;
using AppointmentAuthApi.Repositories;

namespace AppointmentAuthApi.Services
{
    public interface IAppointmentService
    {
        Task<ApiResponse<AppointmentResultDto>> BookSlotAsync(int userId, int slotId);
        Task<ApiResponse<List<SlotDto>>> GetAllSlotsAsync();
        Task<ApiResponse<List<AppointmentDto>>> GetUserAppointmentsAsync(int userId);
        Task<ApiResponse> CancelAppointmentAsync(int appointmentId, int userId);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;
        private readonly IAppointmentRepository _repo;

        public AppointmentService(AppDbContext context, IAppointmentRepository repo)
        {
            _context = context;
            _repo = repo;
        }

        public async Task<ApiResponse<AppointmentResultDto>> BookSlotAsync(int userId, int slotId)
        {
            try
            {
                // Validate user exists
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    return new ApiResponse<AppointmentResultDto>
                    {
                        Success = false,
                        Message = "User not found",
                        Error = "Invalid user ID"
                    };

                // Validate slot exists
                var slot = await _context.Slots.FindAsync(slotId);
                if (slot == null)
                    return new ApiResponse<AppointmentResultDto>
                    {
                        Success = false,
                        Message = "Slot not found",
                        Error = "Invalid slot ID"
                    };

                // Check if slot is already booked
                var exists = await _context.Appointments
                    .AnyAsync(a => a.SlotID == slotId && a.Status == "Booked");

                if (exists)
                    return new ApiResponse<AppointmentResultDto>
                    {
                        Success = false,
                        Message = "Slot already booked",
                        Error = "This slot has been booked by another user"
                    };

                var appointment = new Appointment
                {
                    UserID = userId,
                    SlotID = slotId,
                    Status = "Booked",
                    CreatedAt = DateTime.Now
                };

                await _repo.BookSlotAsync(appointment);
                return new ApiResponse<AppointmentResultDto>
                {
                    Success = true,
                    Message = "Appointment booked successfully",
                    Data = new AppointmentResultDto
                    {
                        AppointmentID = appointment.AppointmentID,
                        SlotStartTime = slot.StartTime.ToString(),
                        SlotEndTime = slot.EndTime.ToString(),
                        Status = appointment.Status,
                        BookedAt = appointment.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AppointmentResultDto>
                {
                    Success = false,
                    Message = "Error booking appointment",
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResponse<List<SlotDto>>> GetAllSlotsAsync()
        {
            try
            {
                var slots = await _repo.GetAllSlotsAsync();
                return new ApiResponse<List<SlotDto>>
                {
                    Success = true,
                    Message = $"Retrieved {slots.Count} available slots",
                    Data = slots.Select(s => new SlotDto
                    {
                        SlotID = s.SlotID,
                        StartTime = s.StartTime.ToString(),
                        EndTime = s.EndTime.ToString(),
                        CreatedBy = s.CreatedBy,
                        IsBooked = s.Appointment != null
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<SlotDto>>
                {
                    Success = false,
                    Message = "Error retrieving slots",
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResponse<List<AppointmentDto>>> GetUserAppointmentsAsync(int userId)
        {
            try
            {
                var appointments = await _repo.GetUserAppointmentsAsync(userId);
                return new ApiResponse<List<AppointmentDto>>
                {
                    Success = true,
                    Message = $"Retrieved {appointments.Count} appointments",
                    Data = appointments.Select(a => new AppointmentDto
                    {
                        AppointmentID = a.AppointmentID,
                        SlotStartTime = a.Slot!.StartTime.ToString(),
                        SlotEndTime = a.Slot.EndTime.ToString(),
                        Status = a.Status,
                        BookedAt = a.CreatedAt
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AppointmentDto>>
                {
                    Success = false,
                    Message = "Error retrieving appointments",
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResponse> CancelAppointmentAsync(int appointmentId, int userId)
        {
            try
            {
                var result = await _repo.CancelAppointmentAsync(appointmentId, userId);
                if (!result)
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Appointment not found or unauthorized",
                        Error = "You can only cancel your own appointments"
                    };

                return new ApiResponse
                {
                    Success = true,
                    Message = "Appointment cancelled successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Error cancelling appointment",
                    Error = ex.Message
                };
            }
        }
    }
}

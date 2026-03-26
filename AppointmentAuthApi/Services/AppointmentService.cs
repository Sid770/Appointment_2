using AppointmentAuthApi.Data;
using Microsoft.EntityFrameworkCore;
using AppointmentAuthApi.Models;
using AppointmentAuthApi.Repositories;

namespace AppointmentAuthApi.Services
{
    public interface IAppointmentService
    {
        Task<string> BookSlotAsync(int userId, int slotId);
        Task<List<Slot>> GetAllSlotsAsync();
        Task<List<Appointment>> GetUserAppointmentsAsync(int userId);
        Task<bool> CancelAppointmentAsync(int appointmentId, int userId);
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

        public async Task<string> BookSlotAsync(int userId, int slotId)
        {
            var exists = await _context.Appointments
                .AnyAsync(a => a.SlotID == slotId && a.Status == "Booked");

            if (exists)
                return "Slot already booked";

            var appointment = new Appointment
            {
                UserID = userId,
                SlotID = slotId,
                Status = "Booked",
                CreatedAt = DateTime.Now
            };

            await _repo.BookSlotAsync(appointment);
            return "Booked Successfully";
        }

        public async Task<List<Slot>> GetAllSlotsAsync()
            => await _repo.GetAllSlotsAsync();

        public async Task<List<Appointment>> GetUserAppointmentsAsync(int userId)
            => await _repo.GetUserAppointmentsAsync(userId);

        public async Task<bool> CancelAppointmentAsync(int appointmentId, int userId)
            => await _repo.CancelAppointmentAsync(appointmentId, userId);
    }
}

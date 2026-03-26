using Microsoft.EntityFrameworkCore;
using AppointmentAuthApi.Data;
using AppointmentAuthApi.Models;

namespace AppointmentAuthApi.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Appointment> BookSlotAsync(Appointment appointment);
        Task<List<Slot>> GetAllSlotsAsync();
        Task<List<Appointment>> GetUserAppointmentsAsync(int userId);
        Task<bool> CancelAppointmentAsync(int appointmentId, int userId);
    }

    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment> BookSlotAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<List<Slot>> GetAllSlotsAsync()
        {
            return await _context.Slots
                .Include(s => s.Appointment)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetUserAppointmentsAsync(int userId)
        {
            return await _context.Appointments
                .Include(a => a.Slot)
                .Where(a => a.UserID == userId && a.Status == "Booked")
                .ToListAsync();
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId, int userId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentID == appointmentId && a.UserID == userId);

            if (appointment == null) return false;

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

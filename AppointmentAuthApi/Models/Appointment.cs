namespace AppointmentAuthApi.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public int UserID { get; set; }
        public User? User { get; set; }
        public int SlotID { get; set; }
        public Slot? Slot { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

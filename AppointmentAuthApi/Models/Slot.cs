namespace AppointmentAuthApi.Models
{
    public class Slot
    {
        public int SlotID { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? CreatedBy { get; set; }
        public Appointment? Appointment { get; set; }
    }
}

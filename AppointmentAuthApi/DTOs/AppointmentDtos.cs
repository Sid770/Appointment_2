namespace AppointmentAuthApi.DTOs
{
    public class SlotDto
    {
        public int SlotID { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public bool IsBooked { get; set; }
    }

    public class AppointmentDto
    {
        public int AppointmentID { get; set; }
        public string SlotStartTime { get; set; } = string.Empty;
        public string SlotEndTime { get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateTime BookedAt { get; set; }
    }

    public class AppointmentResultDto
    {
        public int AppointmentID { get; set; }
        public string SlotStartTime { get; set; } = string.Empty;
        public string SlotEndTime { get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateTime BookedAt { get; set; }
    }
}

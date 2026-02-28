namespace ConferenceRoomBookingSystem
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public RoomDto Room { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public string BookedBy { get; set; }
        public int Capacity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
    }

    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
    }
}
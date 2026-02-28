namespace ConferenceRoomBookingSystem.DTOs
{
    public class BookingListDto
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string BookedBy { get; set; } = string.Empty;
    }

}
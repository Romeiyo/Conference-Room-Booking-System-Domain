namespace ConferenceRoomBookingSystem
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public RoomDto Room { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
    }

    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string Type { get; set; }
    }
}
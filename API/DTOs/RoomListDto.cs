namespace ConferenceRoomBookingSystem.DTOs
{
    public class RoomListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
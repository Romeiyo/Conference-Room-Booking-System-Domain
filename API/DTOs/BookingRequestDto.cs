using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem
{
    public record BookingRequestDto
    {
        public ConferenceRoom Room { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
    }
}
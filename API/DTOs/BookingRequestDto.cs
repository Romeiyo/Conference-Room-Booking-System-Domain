using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem
{
    public record BookingRequestDto
    {
        public ConferenceRoom Room { get; init; }
        public DateOnly BookingDate {get; init; }
        public TimeOnly StartTime { get; init; }
        public TimeOnly EndTime { get; init; }
    }
}
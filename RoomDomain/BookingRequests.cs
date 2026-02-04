using System;

namespace ConferenceRoomBookingSystem
{
    public record BookingRequest
    {
        public ConferenceRoom Room { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }

        public BookingRequest(ConferenceRoom room, DateTime startTime, DateTime endTime)
        {
            Room = room;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
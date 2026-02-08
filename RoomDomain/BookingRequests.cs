using System;

namespace ConferenceRoomBookingSystem
{
    public record BookingRequest
    {
        public ConferenceRoom Room { get; init; }
        public int UserId { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }

        public BookingRequest(ConferenceRoom room, int userId, DateTime startTime, DateTime endTime)
        {
            Room = room;
            UserId = userId;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
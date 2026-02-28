using System;

namespace ConferenceRoomBookingSystem
{
    public record BookingRequest
    {
        public ConferenceRoom Room { get; init; }
        public int UserId { get; init; }
        public string BookedBy {get; init; }
        public DateOnly BookingDate {get; init;}
        public TimeOnly StartTime { get; init; }
        public TimeOnly EndTime { get; init; }

        public BookingRequest(ConferenceRoom room, int userId, string bookedBy, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime)
        {
            Room = room;
            UserId = userId;
            BookedBy = bookedBy;
            BookingDate = bookingDate;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
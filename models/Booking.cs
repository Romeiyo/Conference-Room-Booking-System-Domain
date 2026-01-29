using System;
using ConferenceRoomBookingSystem.Enums;
using ConferenceRoomBookingSystem.Models;

[Serializable]
public class Booking
{
    public ConferenceRoom Room { get; }
    public int UserId { get; }
    public DateTime StartTime { get; set;}
    public DateTime EndTime { get; set;}
    public BookingStatus Status { get; set;} = BookingStatus.Booked;
    public Booking(ConferenceRoom room, int userId, DateTime startTime, DateTime endTime)
    {
        if (room == null) throw new ArgumentException("Room must be a Provided");
        if (userId <= 0) throw new ArgumentException("User ID must be positive");
        if (endTime <= startTime) throw new ArgumentException("End time must be after start time");
        
        Room = room;
        UserId = userId;
        StartTime = startTime;
        EndTime = endTime;
        Status = BookingStatus.Booked;
    }

    public Booking() {}

    public void CancelBooking()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking is already cancelled");
        
        Status = BookingStatus.Cancelled;
        Room.ReleaseRoom();
    }
    
    public bool OverlapsWith(DateTime otherStart, DateTime otherEnd)
    {
        return (StartTime < otherEnd && otherStart < EndTime);
    }
}
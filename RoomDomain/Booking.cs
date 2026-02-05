using System;
namespace ConferenceRoomBookingSystem;
[Serializable]
public class Booking
{
    private static int _nextId = 1;
    public int Id { get; private set; } 
    public ConferenceRoom Room { get; }
    //public int UserId { get; }
    public DateTime StartTime { get; set;}
    public DateTime EndTime { get; set;}
    public BookingStatus Status { get; set;}
    public Booking(ConferenceRoom room,  DateTime startTime, DateTime endTime)
    {
        // if (room == null) throw new ArgumentException("Room must be a Provided");
        // if (userId <= 0) throw new ArgumentException("User ID must be positive");
        // if (endTime <= startTime) throw new ArgumentException("End time must be after start time");
        
        Id = _nextId++;
        Room = room;
        //UserId = userId;
        StartTime = startTime;
        EndTime = endTime;
        Status = BookingStatus.Booked;
    }

    //public Booking() {}
    public void ConfirmBooking()
    {
        Status = BookingStatus.Confirmed;
    }

    public Booking CancelBooking()
    {
        Status = BookingStatus.Cancelled;
        return this;
    }
    
    public bool OverlapsWith(DateTime otherStart, DateTime otherEnd)
    {
        return (StartTime < otherEnd && otherStart < EndTime);
    }
}
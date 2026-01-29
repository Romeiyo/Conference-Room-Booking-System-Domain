using System;
using ConferenceRoomBookingSystem.Enums;
using ConferenceRoomBookingSystem.Models;

namespace ConferenceRoomBookingSystem.Models;
public class Booking
{
    public ConferenceRoom Room { get; }
    /// <summary>
    //public int UserId { get; }
    /// </summary>
    public DateTime Start { get;}
    public DateTime End { get;}
    public BookingStatus Status { get; private set;}
    public Booking(ConferenceRoom room, DateTime start, DateTime end)
    {
       // if (room == null) throw new ArgumentException("Room must be a Provided");
       // if (userId <= 0) throw new ArgumentException("User ID must be positive");
       // if (end <= start) throw new ArgumentException("End time must be after start time");
        
        Room = room;
        //UserId = userId;
        Start = start;
        End = end;
        Status = BookingStatus.Booked;
    }

    //public Booking() {}

    public void Confirm()
    {
        Status = BookingStatus.Confirmed;
    }
    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
    }
    
//     public bool OverlapsWith(DateTime otherStart, DateTime otherEnd)
//     {
//         return (StartTime < otherEnd && otherStart < EndTime);
//     }
}
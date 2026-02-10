using System;
namespace ConferenceRoomBookingSystem;
[Serializable]
public class Booking
{
    public int Id { get; set; } 
    public int RoomId { get; set; }
    public ConferenceRoom? Room { get; set; }
    public int UserId { get; set; }
    public DateTime StartTime { get; set;}
    public DateTime EndTime { get; set;}
    public BookingStatus Status { get; set;}
    public int Capacity { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAt { get; set; } 
    public Booking(ConferenceRoom room, int userId,  DateTime startTime, DateTime endTime)
    {   
        Room = room ?? throw new ArgumentNullException(nameof(room));
        RoomId = room.Id;
        UserId = userId;
        StartTime = startTime;
        EndTime = endTime;
        Status = BookingStatus.Booked;
        Capacity = room.Capacity;
        CreatedAt = DateTime.UtcNow;
        CancelledAt = null;
    }

    public Booking()
    {
        CreatedAt = DateTime.UtcNow;
    }
    public void ConfirmBooking()
    {
        Status = BookingStatus.Confirmed;
    }

    public void CancelBooking()
    {
        Status = BookingStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }
    
    public bool OverlapsWith(DateTime otherStart, DateTime otherEnd)
    {
        return (StartTime < otherEnd && otherStart < EndTime);
    }
}
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
    public int Capacity { get; set; } //This is non nullable because every booking must have a
                                      //capacity, which is determined by the room's capacity at
                                      //the time of booking. This allows us to maintain a record
                                      //of the room's capacity at the time of booking, even if the
                                      //room's capacity changes in the future.
    public DateTime? CreatedAt { get; set; } //This is nullable because the exixting bookings 
                                             //would not have this field populated, and we want
                                             //to avoid breaking changes. For new bookings, this 
                                             //will be set to the current UTC time when the booking 
                                             //is created.
    public DateTime? CancelledAt { get; set; } //This is nullable because only cancelled bookings 
                                               //will have this field populated. For non-cancelled 
                                               //bookings, this will be null.
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
        CreatedAt = DateTime.UtcNow; //New bookings will have this set to the current UTC time, 
                                     //while existing bookings will have this as null. This allows
                                     //us to avoid breaking changes while still populating
                                     //this field for new bookings.
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
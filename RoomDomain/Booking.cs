using System;
namespace ConferenceRoomBookingSystem;
[Serializable]
public class Booking
{
    public int Id { get; set; } 
    public int RoomId { get; set; }
    public ConferenceRoom? Room { get; set; }
    public int UserId { get; set; }
    public string BookedBy {get; set;} = string.Empty;
    public DateOnly BookingDate {get; set;}
    public TimeOnly StartTime { get; set;}
    public TimeOnly EndTime { get; set;}
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
    public Booking(ConferenceRoom room, int userId, string bookedBy, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime)
    {   
        Room = room ?? throw new ArgumentNullException(nameof(room));
        RoomId = room.Id;
        UserId = userId;
        BookedBy = bookedBy;
        BookingDate = bookingDate;
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

    // Helper method to get full DateTime for compatibility if needed
    public DateTime GetStartDateTime() => BookingDate.ToDateTime(StartTime);
    public DateTime GetEndDateTime() => BookingDate.ToDateTime(EndTime);
    public void ConfirmBooking()
    {
        Status = BookingStatus.Booked;
    }

    public void CancelBooking()
    {
        Status = BookingStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
    }
    
    public bool OverlapsWith(DateOnly otherDate, TimeOnly otherStart, TimeOnly otherEnd)
    {
        if (BookingDate != otherDate) return false;
        return (StartTime < otherEnd && otherStart < EndTime);
    }
}
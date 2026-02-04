using ConferenceRoomBookingSystem.Enums;
using ConferenceRoomBookingSystem.Models;
namespace ConferenceRoomBookingSystem.Logic;
public class BookingManager //Central Business Logic/ All  the business rules are here
{
    //Properties
    private readonly List<Booking> _bookings;

    //Methods
    public IReadOnlyList<Booking> GetBookings()
    {
        return _bookings.ToList();
    }

    public Booking CreateBooking(BookingRequest request)
    {
        //Guard Clauses
        if(request.Room == null)
        {
            throw new ArgumentException("Room must exist");
        }
        
        if(request.Start >= request.End)
        {
            throw new ArgumentException("Invalid time range");
        }

        bool overlaps = _bookings.Any(b => b.Room == request.Room && b.Status == BookingStatus.Confirmed && request.Start < b.End && request.End > b.Start);

        if(overlaps)
        {
            throw new BookingConflictException();
        }

        Booking booking = new Booking(request.Room, request.Start, request.End);
        
        booking.Confirm();
        _bookings.Add(booking);
        
        return booking;

    }

}
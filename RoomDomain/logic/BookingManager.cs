namespace ConferenceRoomBookingSystem
{
    public class BookingManager
    {
        //properties
        private readonly List<Booking> _bookings;

        public BookingManager()
        {
            _bookings = new List<Booking>();
        }

        //methods
        public IReadOnlyList<Booking> GetBookings()
        {
            return _bookings.ToList();
        }

        public Booking CreateBooking(BookingRequest request)
        {
            if(request.Room == null)
            {
                throw new ArgumentException("Room must be provided");
            }
            if(request.StartTime >= request.EndTime)
            {
                throw new ArgumentException("End time must be after start time");
            }
            bool overlaps = _bookings.Any(b => b.Room.Id == request.Room.Id && b.Status == BookingStatus.Confirmed && request.StartTime < b.EndTime && request.EndTime > b.StartTime);

            if(overlaps)
            {
                throw new BookingConflictException();
            }

            Booking booking = new Booking(request.Room, request.StartTime, request.EndTime);

            booking.ConfirmBooking();
            _bookings.Add(booking);

            return booking;
        }
    }
}
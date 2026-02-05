namespace ConferenceRoomBookingSystem
{
    public class BookingManager
    {
        //properties
        private readonly List<Booking> _bookings;
        private readonly IBookingStore _bookingStore;

        public BookingManager(IBookingStore bookingStore)
        {
            // var storedBookings = await _bookingStore.LoadBookingsAsync();
            _bookingStore = bookingStore;
            _bookings = new List<Booking>();
            LoadBookingsFromStore();
        }

        private async void LoadBookingsFromStore()
        {
            try
            {
                var storedBookings = await _bookingStore.LoadBookingsAsync();
                _bookings.Clear();
                _bookings.AddRange(storedBookings);
            }
            catch (Exception ex)
            {
                // Log the exception in a real application
                Console.WriteLine($"Error loading bookings: {ex.Message}");
            }
        }

        //methods
        public IReadOnlyList<Booking> GetBookings()
        {
            return _bookings.ToList().AsReadOnly();
        }

        // public Booking CreateBooking(BookingRequest request)
        // {
        //     if(request.Room == null)
        //     {
        //         throw new ArgumentException("Room must be provided");
        //     }
        //     if(request.StartTime >= request.EndTime)
        //     {
        //         throw new ArgumentException("End time must be after start time");
        //     }
        //     bool overlaps = _bookings.Any(b => b.Room.Id == request.Room.Id && b.Status == BookingStatus.Confirmed && request.StartTime < b.EndTime && request.EndTime > b.StartTime);

        //     if(overlaps)
        //     {
        //         throw new BookingConflictException();
        //     }

        //     Booking booking = new Booking(request.Room, request.StartTime, request.EndTime);

        //     booking.ConfirmBooking();
        //     _bookings.Add(booking);

        //     return booking;
        // }

        public async Task<Booking> CreateBookingAsync(BookingRequest request)
        {
            if (request.Room == null)
            {
                throw new ArgumentException("Room must be provided");
            }
            
            if (request.StartTime >= request.EndTime)
            {
                throw new ArgumentException("End time must be after start time");
            }

            bool overlaps = _bookings.Any(b => 
                b.Room.Id == request.Room.Id && 
                b.Status == BookingStatus.Confirmed && 
                request.StartTime < b.EndTime && 
                request.EndTime > b.StartTime);

            if (overlaps)
            {
                throw new BookingConflictException();
            }

            Booking booking = new Booking(request.Room, request.StartTime, request.EndTime);
            booking.ConfirmBooking();
            
            _bookings.Add(booking);
            
            // Save to store
            await SaveBookingsToStore();
            
            return booking;
        }

        public async Task<bool> CancelBookingAsync(Booking booking)
        {
            if (booking != null && _bookings.Contains(booking))
            {
                booking.CancelBooking();
                await SaveBookingsToStore();
                return true;
            }
            return false;
        }

        private async Task SaveBookingsToStore()
        {
            try
            {
                await _bookingStore.SaveBookingsAsync(_bookings);
            }
            catch (Exception ex)
            {
                // Log the exception in a real application
                Console.WriteLine($"Error saving bookings: {ex.Message}");
                throw;
            }
        }
    }
}
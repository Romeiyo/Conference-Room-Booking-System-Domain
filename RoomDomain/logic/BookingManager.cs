namespace ConferenceRoomBookingSystem
{
    public class BookingManager
    {
        //properties
        private readonly List<Booking> _bookings;
        private readonly IBookingStore _bookingStore;
        private readonly RoomRepository _roomRepository;

        public BookingManager(IBookingStore bookingStore, RoomRepository roomRepository)
        {
            // var storedBookings = await _bookingStore.LoadBookingsAsync();
            _bookingStore = bookingStore;
            _roomRepository = roomRepository;
            _bookings = new List<Booking>();
            LoadBookingsFromStore();
        }

        // private async void LoadBookingsFromStore()
        // {
        //     try
        //     {
        //         var storedBookings = await _bookingStore.LoadBookingsAsync();
        //         _bookings.Clear();
        //         _bookings.AddRange(storedBookings);
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log the exception in a real application
        //         Console.WriteLine($"Error loading bookings: {ex.Message}");
        //     }
        // }

        private async void LoadBookingsFromStore()
        {
            try
            {
                var storedBookings = await _bookingStore.LoadBookingsAsync();
                _bookings.Clear();
                
                foreach (var booking in storedBookings)
                {
                    if (booking.Room != null)
                    {
                        var room = _roomRepository.GetRoomById(booking.Room.Id);
                        if (room != null)
                        {
                            var restoredBooking = new Booking(room, booking.StartTime, booking.EndTime)
                            {
                                Status = booking.Status
                            };
                            _bookings.Add(restoredBooking);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
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
            //var room = _roomRepository.GetRoomById(request.RoomId);
            
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
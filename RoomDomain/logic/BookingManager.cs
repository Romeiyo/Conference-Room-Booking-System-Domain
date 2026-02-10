using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem
{
    public class BookingManager
    {
        //properties
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;

        public BookingManager(IBookingRepository bookingRepository, IRoomRepository roomRepository)
        {
            // var storedBookings = await _bookingStore.LoadBookingsAsync();
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
        }

        // private async void LoadBookingsFromStore()
        // {
        //     try
        //     {
        //         var storedBookings = await _bookingStore.LoadBookingsAsync();
        //         _bookings.Clear();
                
        //         foreach (var booking in storedBookings)
        //         {
        //             if (booking.Room != null)
        //             {
        //                 var room = _roomRepository.GetRoomById(booking.Room.Id);
        //                 if (room != null)
        //                 {
        //                     var restoredBooking = new Booking(room, booking.UserId, booking.StartTime, booking.EndTime)
        //                     {
        //                         Status = booking.Status
        //                     };
        //                     _bookings.Add(restoredBooking);
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error loading bookings: {ex.Message}");
        //     }
        // }

        //methods
        public async Task<IReadOnlyList<Booking>> GetBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.ToList().AsReadOnly();
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            return await _bookingRepository.GetByIdAsync(bookingId);
        }

        public async Task<IReadOnlyList<Booking>> GetUserBookingsAsync(int userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return bookings.ToList().AsReadOnly();
        }

        public async Task<Booking> CreateBookingAsync(BookingRequest request)
        {
            //Validating if room exists and is valid
            var room = await _roomRepository.GetByIdAsync(request.Room.Id);
            if (room == null)
            {
                throw new ArgumentException("Room must be provided");
            }
            
            //validating booking times
            if (request.StartTime >= request.EndTime)
            {
                throw new ArgumentException("End time must be after start time");
            }

            bool overlaps = await _bookingRepository.HasOverlapAsync(
                room.Id, 
                request.StartTime, 
                request.EndTime);

            if (overlaps)
            {
                throw new BookingConflictException();
            }

            // Create and confirm the booking
            var booking = new Booking(room, request.UserId, request.StartTime, request.EndTime);

            booking.ConfirmBooking();
            
            var createdBooking = await _bookingRepository.AddAsync(booking);
            
            return createdBooking;
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                return false; // Booking not found
            }

            //check if already cancelled
            if (booking.Status == BookingStatus.Cancelled)
            {
                return false; // Already cancelled
            }

            booking.CancelBooking();
            
            await _bookingRepository.UpdateAsync(booking);
            
            return true;
        }

        // private async Task SaveBookingsToStore()
        // {
        //     try
        //     {
        //         await _bookingStore.SaveBookingsAsync(_bookings);
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log the exception in a real application
        //         Console.WriteLine($"Error saving bookings: {ex.Message}");
        //         throw;
        //     }
        // }

        public async Task<IReadOnlyList<ConferenceRoom>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return rooms.ToList().AsReadOnly();
        }

        public async Task<ConferenceRoom> GetRoomByIdAsync(int roomId)
        {
            return await _roomRepository.GetByIdAsync(roomId);
        }

    }
}
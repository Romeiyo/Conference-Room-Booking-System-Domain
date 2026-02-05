using ConferenceRoomBookingSystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConferenceRoomBookingSystem
{
    public interface IBookingStore
    {
        Task SaveBookingsAsync(IEnumerable<Booking> bookings);
        Task<IReadOnlyList<Booking>> LoadBookingsAsync();
    }
}
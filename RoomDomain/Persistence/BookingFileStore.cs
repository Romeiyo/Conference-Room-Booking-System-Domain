using System;
using System.Text.Json;
using System.Threading.Tasks;
using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem.Persistence
{
    public class BookingFileStore : IBookingStore
    {
        private readonly string _filePath;

        public BookingFileStore(string filePath)
        {
            _filePath = filePath;
        }

        public async Task SaveBookingsAsync(IEnumerable<Booking> bookings)
        {
            // if(!Directory.Exists(_directoryPath))
            // {
            //     Directory.CreateDirectory(_directoryPath);
            // }
            // var existingBookings = (await LoadBookingsAsync()).ToList();
            // existingBookings.Add(bookings);

            var json = JsonSerializer.Serialize(bookings);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<IReadOnlyList<Booking>> LoadBookingsAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Booking>();
            }

            string json = await File.ReadAllTextAsync(_filePath);
            var bookings = JsonSerializer.Deserialize<List<Booking>>(json) ?? new List<Booking>();
            return bookings.AsReadOnly();
        }
    }
}
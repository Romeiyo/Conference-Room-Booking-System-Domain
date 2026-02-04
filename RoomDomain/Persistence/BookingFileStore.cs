using System;
using System.Text.Json;
using System.Threading.Tasks;
using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem.Persistence
{
    public class BookingFileStore
    {
        private readonly string _filePath;

        public BookingFileStore(string filePath)
        {
            _filePath = filePath;
        }

        public async Task SaveBookingsAsync(IEnumerable<Booking> bookings)
        {
            string json = JsonSerializer.Serialize(bookings);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<List<Booking>> LoadBookingsAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Booking>();
            }

            string json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<Booking>>(json) ?? new List<Booking>();
        }
    }
}
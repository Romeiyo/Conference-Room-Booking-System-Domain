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
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyJsonConverter(), new TimeOnlyJsonConverter() }
            };

            var json = JsonSerializer.Serialize(bookings, options);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<IReadOnlyList<Booking>> LoadBookingsAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Booking>();
            }

            string json = await File.ReadAllTextAsync(_filePath);
            var options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyJsonConverter(), new TimeOnlyJsonConverter() }
            };
            var bookings = JsonSerializer.Deserialize<List<Booking>>(json) ?? new List<Booking>();
            return bookings.AsReadOnly();
        }
    }

     public class DateOnlyJsonConverter : System.Text.Json.Serialization.JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateOnly.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
        }
    }

    public class TimeOnlyJsonConverter : System.Text.Json.Serialization.JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeOnly.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("HH:mm:ss"));
        }
    }
}
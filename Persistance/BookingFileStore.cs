using System.Text.Json;
using ConferenceRoomBookingSystem.Enums;
using ConferenceRoomBookingSystem.Models;

public class BookingFileStore
{
    private readonly string _filePath;
    public BookingFileStore(string _filePath)
    {
        _filePath = _filePath;
    }

    public async Task SaveAsync(IEnumerable<Booking> bookings)
    {
        string json = JsonSerializer.Serialize(bookings);
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<List<Booking>> LoadAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Booking>();
        }

        string json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<Booking>>(json) ?? new List<Booking>();
    }
}
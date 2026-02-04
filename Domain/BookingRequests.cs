
using ConferenceRoomBookingSystem.Models;

public record BookingRequest
{
    public ConferenceRoom Room { get;}
    public DateTime Start { get; }
    public DateTime End { get;}
    //public ConferenceRoom ConferenceRoom { get; }
    //public DateTime Now { get; }
    //public DateTime DateTime { get; }

    public BookingRequest(ConferenceRoom room, DateTime start, DateTime end)
    {
        Room = room;
        Start = start;
        End = end;
    }

    // public BookingRequest(ConferenceRoom room, DateTime now, DateTime dateTime)
    // {
    //     Room = room;
    //     Now = now;
    //     DateTime = dateTime;
    // }
}
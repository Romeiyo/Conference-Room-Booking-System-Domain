
using ConferenceRoomBookingSystem.Models;

public record BookingRequest
{
    public ConferenceRoom Room { get;}
    public DateTime Start { get; }
    public DateTime End { get;}
    public ConferenceRoom ConferenceRoom { get; }
    public DateTime Now { get; }
    public DateTime DateTime { get; }

    public BookingRequest(ConferenceRoom Room, DateTime Start, DateTime End)
    {
        Room = Room;
        Start = Start;
        End = End;
    }

    public BookingRequest(ConferenceRoom conferenceRoom, DateTime now, DateTime dateTime)
    {
        ConferenceRoom = conferenceRoom;
        Now = now;
        DateTime = dateTime;
    }
}
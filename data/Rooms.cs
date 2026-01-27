using System;
using System.Collections.Generic;

public static class Rooms{
    public static List<ConferenceRoom> ConferenceRooms {get; } = new List<ConferenceRoom>
    {
        new ConferenceRoom (1, "Room A", 10, true),
        new ConferenceRoom (2, "Room B", 20, true),
        new ConferenceRoom (3, "Room C", 15, false),
        new ConferenceRoom (4, "Room D", 25, true),
        new ConferenceRoom (5, "Room E", 30, false),
        new ConferenceRoom (6, "Room F", 10, false),
        new ConferenceRoom (7, "Room G", 20, true),
        new ConferenceRoom (8, "Room H", 15, true),
        new ConferenceRoom (9, "Room I", 13, true),
        new ConferenceRoom (10, "Room J", 20, true),
        new ConferenceRoom (11, "Room K", 10, false),
        new ConferenceRoom (12, "Room L", 5, false),
        new ConferenceRoom (13, "Room M", 12, true),
        new ConferenceRoom (14, "Room N", 15, false),
        new ConferenceRoom (15, "Room O", 12, true),
        new ConferenceRoom (16, "Room P", 30, false),
    };
}
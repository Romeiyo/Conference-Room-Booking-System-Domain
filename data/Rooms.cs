using System;
using System.Collections.Generic;
using ConferenceRoomBookingSystem.Enums;
using ConferenceRoomBookingSystem.Models;

public static class Rooms{
    public static List<ConferenceRoom> ConferenceRooms {get; } = new List<ConferenceRoom>
    {
        new ConferenceRoom (1, "Room A", 10, true, RoomType.Standard),
        new ConferenceRoom (2, "Room B", 20, true, RoomType.Boardroom),
        new ConferenceRoom (3, "Room C", 15, false, RoomType.Training),
        new ConferenceRoom (4, "Room D", 25, true, RoomType.Standard),
        new ConferenceRoom (5, "Room E", 30, false, RoomType.Boardroom),
        new ConferenceRoom (6, "Room F", 10, false, RoomType.Training),
        new ConferenceRoom (7, "Room G", 20, true, RoomType.Standard),
        new ConferenceRoom (8, "Room H", 15, true, RoomType.Boardroom),
        new ConferenceRoom (9, "Room I", 13, true, RoomType.Training),
        new ConferenceRoom (10, "Room J", 20, true, RoomType.Standard),
        new ConferenceRoom (11, "Room K", 10, false, RoomType.Boardroom),
        new ConferenceRoom (12, "Room L", 5, false, RoomType.Training),
        new ConferenceRoom (13, "Room M", 12, true, RoomType.Standard),
        new ConferenceRoom (14, "Room N", 15, false, RoomType.Boardroom),
        new ConferenceRoom (15, "Room O", 12, true, RoomType.Training),
        new ConferenceRoom (16, "Room P", 30, false, RoomType.Standard),
    };
}
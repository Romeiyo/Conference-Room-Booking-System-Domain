using System;
using System.Collections.Generic;
using ConferenceRoomBookingSystem.Enums;
using ConferenceRoomBookingSystem.Models;

namespace ConferenceRoomBookingSystem.Data;
public class SeedData{
    public List<ConferenceRoom> SeedRooms()
    {
        List<ConferenceRoom> ConferenceRooms = new List<ConferenceRoom>
        {

            new ConferenceRoom (1, "Room A", 10, RoomType.Standard),
            new ConferenceRoom (2, "Room B", 20, RoomType.Boardroom),
            new ConferenceRoom (3, "Room C", 15, RoomType.Training),
            new ConferenceRoom (4, "Room D", 25, RoomType.Standard),
            new ConferenceRoom (5, "Room E", 30, RoomType.Boardroom),
            new ConferenceRoom (6, "Room F", 10, RoomType.Training),
            new ConferenceRoom (7, "Room G", 20, RoomType.Standard),
            new ConferenceRoom (8, "Room H", 15, RoomType.Boardroom),
            new ConferenceRoom (9, "Room I", 13, RoomType.Training),
            new ConferenceRoom (10, "Room J", 20, RoomType.Standard),
            new ConferenceRoom (11, "Room K", 10, RoomType.Boardroom),
            new ConferenceRoom (12, "Room L", 5, RoomType.Training),
            new ConferenceRoom (13, "Room M", 12, RoomType.Standard),
            new ConferenceRoom (14, "Room N", 15, RoomType.Boardroom),
            new ConferenceRoom (15, "Room O", 12, RoomType.Training),
            new ConferenceRoom (16, "Room P", 30, RoomType.Standard),
        };

    return ConferenceRooms;
    }
    
}
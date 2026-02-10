using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem
{
    public class SeedData(){

        public List<ConferenceRoom> SeedRooms()
        {
            List<ConferenceRoom> ConferenceRooms = new List<ConferenceRoom>()
            {   
                new ConferenceRoom (1, "Room A", 10, RoomType.Standard, "Bloemfontein", true),
                new ConferenceRoom (2, "Room B", 20, RoomType.Boardroom, "Bloemfontein", true),
                new ConferenceRoom (3, "Room C", 15, RoomType.Training, "Bloemfontein", true),
                new ConferenceRoom (4, "Room D", 25, RoomType.Standard, "Bloemfontein", true),
                new ConferenceRoom (5, "Room E", 30, RoomType.Boardroom, "Bloemfontein", true),
                new ConferenceRoom (6, "Room F", 10, RoomType.Training, "Bloemfontein", false), // Inactive
                new ConferenceRoom (7, "Room G", 20, RoomType.Standard, "Bloemfontein", true),
                new ConferenceRoom (8, "Room H", 15, RoomType.Boardroom, "Bloemfontein", true),
                new ConferenceRoom (9, "Room I", 13, RoomType.Training, "Cape Town", true),
                new ConferenceRoom (10, "Room J", 20, RoomType.Standard, "Cape Town", true),
                new ConferenceRoom (11, "Room K", 10, RoomType.Boardroom, "Bloemfontein", false), // Inactive
                new ConferenceRoom (12, "Room L", 5, RoomType.Training, "Cape Town", true),
                new ConferenceRoom (13, "Room M", 12, RoomType.Standard, "Bloemfontein", true),
                new ConferenceRoom (14, "Room N", 15, RoomType.Boardroom, "Bloemfontein", true),
                new ConferenceRoom (15, "Room O", 12, RoomType.Training, "Cape Town", false), // Inactive
                new ConferenceRoom (16, "Room P", 30, RoomType.Standard, "Cape Town", true),
            };
            return ConferenceRooms;
        }    
    }
}

using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem
{
    public class SeedData()
    {
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

        public List<Booking> SeedBookings()
        {
            var rooms = SeedRooms();

            List<Booking> Bookings = new List<Booking>()
            {
                new Booking(rooms[0], 2, new DateTime(2026, 02, 10, 9, 0, 0), new DateTime(2026, 02, 10, 10, 0, 0))
                {
                    Id = 1,
                    CreatedAt = new DateTime(2026, 01, 15, 12, 0, 0)
                },

                new Booking(rooms[1], 3, new DateTime(2026, 02, 11, 14, 0, 0), new DateTime(2026, 02, 11, 15, 30, 0))
                {
                    Id = 2,
                    Status = BookingStatus.Cancelled,
                    CreatedAt = new DateTime(2026, 01, 20, 9, 30, 0),
                    CancelledAt = new DateTime(2026, 01, 25, 10, 0, 0)
                },
            };
            return Bookings;
        }    
    }
 
}

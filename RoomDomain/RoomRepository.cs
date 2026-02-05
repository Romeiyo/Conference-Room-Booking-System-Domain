using System;
using System.Collections.Generic;
using System.Linq;

namespace ConferenceRoomBookingSystem
{
    public class RoomRepository
    {
        private readonly List<ConferenceRoom> _rooms;

        public RoomRepository()
        {
            // Initialize with seed data
            var seedData = new SeedData();
            _rooms = seedData.SeedRooms();
        }

        public ConferenceRoom GetRoomById(int roomId)
        {
            return _rooms.FirstOrDefault(r => r.Id == roomId);
        }

        public IReadOnlyList<ConferenceRoom> GetAllRooms()
        {
            return _rooms.AsReadOnly();
        }

        public bool RoomExists(ConferenceRoom room)
        {
            if (room == null) return false;
            
            return _rooms.Any(r => r.Id == room.Id && 
                                  r.Name == room.Name && 
                                  r.Capacity == room.Capacity && 
                                  r.Type == room.Type);
        }

        // Optional: Add a room if it doesn't exist
        // public void AddRoom(ConferenceRoom room)
        // {
        //     if (!_rooms.Any(r => r.Id == room.Id))
        //     {
        //         _rooms.Add(room);
        //     }
        // }
    }
}
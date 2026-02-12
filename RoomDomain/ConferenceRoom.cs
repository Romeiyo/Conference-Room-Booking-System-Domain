using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem{
    [Serializable]
    public class ConferenceRoom{
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoomType Type { get; set; } = RoomType.Standard;
        
        public ConferenceRoom() { }

        public ConferenceRoom(int id, string name, int capacity, RoomType type, string location = "", bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("A room name is required");
            }

            if (capacity <= 0)
            {
                throw new Exception("Room capacity must be a positive number");
            }

            Type = type;
            Id = id;
            Name = name ?? string.Empty;
            Location = location ?? string.Empty;
            IsActive = isActive;
            Capacity = capacity;
            
        }
    }
}
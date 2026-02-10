using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem{
    [Serializable]
    public class ConferenceRoom{
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoomType Type { get; set; } 
        
        public ConferenceRoom() { }

        public ConferenceRoom(int id, string name, int capacity, RoomType type)
        {
            // if (id <= 0) throw new ArgumentException("Id must be positive");
            // if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
            // if (capacity <= 0) throw new ArgumentException("Capacity must be positive");
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
            Name = name;
            Capacity = capacity;
            
        }
    }
}
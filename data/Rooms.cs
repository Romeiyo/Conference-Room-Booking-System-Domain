public static class Rooms{
    public static List<ConferenceRoom> GetAvailableRooms {get; } = new List<ConferenceRoom>
    {
        new ConferenceRoom { Id = 1, Name = "Room A", Capacity = 10, IsAvailable = true },
        new ConferenceRoom { Id = 2, Name = "Room B", Capacity = 20, IsAvailable = true },
        new ConferenceRoom { Id = 3, Name = "Room C", Capacity = 15, IsAvailable = false },
        new ConferenceRoom { Id = 4, Name = "Room D", Capacity = 25, IsAvailable = true },
        new ConferenceRoom { Id = 5, Name = "Room E", Capacity = 30, IsAvailable = false },
        new ConferenceRoom { Id = 6, Name = "Room F", Capacity = 10, IsAvailable = false },
        new ConferenceRoom { Id = 7, Name = "Room G", Capacity = 20, IsAvailable = true },
        new ConferenceRoom { Id = 8, Name = "Room H", Capacity = 15, IsAvailable = true },
        new ConferenceRoom { Id = 9, Name = "Room I", Capacity = 13, IsAvailable = true },
        new ConferenceRoom { Id = 10, Name = "Room J", Capacity = 20, IsAvailable = true },
        new ConferenceRoom { Id = 11, Name = "Room K", Capacity = 10, IsAvailable = false },
        new ConferenceRoom { Id = 12, Name = "Room L", Capacity = 5, IsAvailable = false },
        new ConferenceRoom { Id = 13, Name = "Room M", Capacity = 12, IsAvailable = true },
        new ConferenceRoom { Id = 14, Name = "Room N", Capacity = 15, IsAvailable = false },
        new ConferenceRoom { Id = 15, Name = "Room O", Capacity = 12, IsAvailable = true },
        new ConferenceRoom { Id = 16, Name = "Room P", Capacity = 30, IsAvailable = false },
    };
}
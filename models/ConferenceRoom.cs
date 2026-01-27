public class ConferenceRoom
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }

    public ConferenceRoom(int id, string name, int capacity, bool isAvailable)
    {
        if (id <= 0) throw new ArgumentException("Id must be positive");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (capacity <= 0) throw new ArgumentException("Capacity must be positive");
        
        Id = id;
        Name = name;
        Capacity = capacity;
        IsAvailable = isAvailable;
    }
    
    public void BookRoom()
    {
        if (!IsAvailable)
            throw new InvalidOperationException("Room is not available");
        IsAvailable = false;
    }
    
    public void ReleaseRoom()
    {
        IsAvailable = true;
    }
}
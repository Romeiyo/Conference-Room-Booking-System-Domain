using System;

public class Booking
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Booking(int roomId, int userId, DateTime startTime, DateTime endTime)
    {
        if (roomId <= 0) throw new ArgumentException("Room ID must be positive");
        if (userId <= 0) throw new ArgumentException("User ID must be positive");
        if (endTime <= startTime) throw new ArgumentException("End time must be after start time");
        
        RoomId = roomId;
        UserId = userId;
        StartTime = startTime;
        EndTime = endTime;
    }
    
    public bool OverlapsWith(DateTime otherStart, DateTime otherEnd)
    {
        return (StartTime < otherEnd && otherStart < EndTime);
    }
}
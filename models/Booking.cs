public class Booking
{
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
    public Booking(int roomId, int userId, DateTime startTime, DateTime endTime)
    {
        if (roomId <= 0) throw new ArgumentException("Room ID must be positive");
        if (userId <= 0) throw new ArgumentException("User ID must be positive");
        if (endTime <= startTime) throw new ArgumentException("End time must be after start time");
        
        RoomId = roomId;
        UserId = userId;
        StartTime = startTime;
        EndTime = endTime;
        Status = BookingStatus.Confirmed;
    }
    
    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Booking is already cancelled");
        
        Status = BookingStatus.Cancelled;
    }
    
    public bool OverlapsWith(DateTime otherStart, DateTime otherEnd)
    {
        return (StartTime < otherEnd && otherStart < EndTime);
    }
}
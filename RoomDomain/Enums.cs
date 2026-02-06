namespace ConferenceRoomBookingSystem
{
    public enum BookingStatus
    {
        Booked,
        Cancelled,
        Confirmed,
        InProgress
    }

    public enum RoomType
    {
        Standard,
        Boardroom,
        Training,
    }

    public enum ErrorCategory
    {
        ValidationError,       
        BusinessRuleViolation,  
        InfrastructureFailure, 
        UnexpectedError         
    }
}
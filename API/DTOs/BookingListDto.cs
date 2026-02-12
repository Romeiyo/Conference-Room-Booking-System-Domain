namespace ConferenceRoomBookingSystem.DTOs
{
    public class BookingListDto
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    // public class PaginatedResponse<T>
    // {
    //     public int Page { get; set; }
    //     public int PageSize { get; set; }
    //     public int TotalCount { get; set; }
    //     public int TotalPages { get; set; }
    //     public string? SortBy { get; set; }
    //     public string? SortDirection { get; set; }
    //     public Dictionary<string, string?> AppliedFilters { get; set; } = new();
    //     public IEnumerable<T> Items { get; set; } = new List<T>();
    // }
}
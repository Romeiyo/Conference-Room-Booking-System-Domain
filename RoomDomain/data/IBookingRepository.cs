using System;
using ConferenceRoomBookingSystem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConferenceRoomBookingSystem
{
    public interface IBookingRepository
    {
        Task<Booking> GetByIdAsync(int id);
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
        Task <IEnumerable<Booking>> GetByRoomIdAsync(int roomId);
        Task<Booking> AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> HasOverlapAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
        Task SaveChangesAsync();
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConferenceRoomBookingSystem
{
    public interface IRoomRepository
    {
        Task<ConferenceRoom> GetByIdAsync(int id);
        Task<IEnumerable<ConferenceRoom>> GetAllAsync();
        Task<IEnumerable<ConferenceRoom>> GetActiveRoomsAsync();
        Task<ConferenceRoom> AddAsync(ConferenceRoom room);
        Task UpdateAsync(ConferenceRoom room);
        Task<bool> ExistsAsync(int id);
        Task<bool> RoomExistsAsync(int id, string name, int capacity, RoomType type);
    }
}
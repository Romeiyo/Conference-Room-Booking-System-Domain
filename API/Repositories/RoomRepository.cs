using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBookingSystem
{
    public class RoomRepository : IRoomRepository
    {
        private readonly BookingsDbContext _context;

        public RoomRepository(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<ConferenceRoom> GetByIdAsync(int id)
        {
            return await _context.ConferenceRooms.FindAsync(id);
        }

        public async Task<IEnumerable<ConferenceRoom>> GetAllAsync()
        {
            return await _context.ConferenceRooms
                .OrderBy(r => r.Id)
                .ToListAsync();
        }

        public async Task<ConferenceRoom> AddAsync(ConferenceRoom room)
        {
            _context.ConferenceRooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task UpdateAsync(ConferenceRoom room)
        {
            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ConferenceRooms.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> RoomExistsAsync(int id, string name, int capacity, RoomType type)
        {
            return await _context.ConferenceRooms.AnyAsync(r => 
                r.Id == id && 
                r.Name == name && 
                r.Capacity == capacity && 
                r.Type == type);
        }

        public async Task<IEnumerable<ConferenceRoom>> GetActiveRoomsAsync()
        {
            return await _context.ConferenceRooms
                .Where(r => r.IsActive)  
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}
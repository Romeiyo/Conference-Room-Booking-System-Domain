using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingsDbContext _context;

        public BookingRepository(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByRoomIdAsync(int roomId)
        {
            return await _context.Bookings
                .Where(b => b.RoomId == roomId)
                .OrderBy(b => b.BookingDate)
                .ThenBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<Booking> AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await GetByIdAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Bookings.AnyAsync(b => b.Id == id);
        }

        public async Task<bool> HasOverlapAsync(int roomId, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime, int? excludeBookingId = null)
        {
            var query = _context.Bookings
                .Where(b => b.RoomId == roomId 
                         && b.BookingDate == bookingDate
                         && b.Status == BookingStatus.Confirmed
                         && startTime < b.EndTime 
                         && endTime > b.StartTime);

            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookingId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
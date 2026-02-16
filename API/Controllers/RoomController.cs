using ConferenceRoomBookingSystem;
using ConferenceRoomBookingSystem.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly BookingsDbContext _context;

        public RoomController(BookingsDbContext context)
        {
            _context = context;
        }

        /// GET /api/rooms/rooms/name?page=1&pageSize=10
        [HttpGet("rooms/name")]
        [Authorize(Roles = "Admin,Receptionist,Employee,Facility Manager")]
        public async Task<IActionResult> GetRoomsSortedByName(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.ConferenceRooms
                .Where(r => r.IsActive)
                .AsNoTracking();

            query = query.OrderBy(r => r.Name);
            
            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoomListDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Capacity = r.Capacity,
                    Location = r.Location,
                    Type = r.Type.ToString(),
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                data = results
            });
        }

        /// GET /api/rooms/rooms/capacity?page=1&pageSize=10&sortBy=capacity
        [HttpGet("rooms/capacity")]
        [Authorize(Roles = "Admin,Receptionist,Employee,Facility Manager")]
        public async Task<IActionResult> GetRoomsSortedByCapacity(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var query = _context.ConferenceRooms
                .Where(r => r.IsActive)
                .AsNoTracking();

            if (sortBy == "capacity")
            {
                query = query.OrderBy(r => r.Capacity);
            }
            else
            {
                query = query.OrderByDescending(r => r.Capacity);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoomListDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Capacity = r.Capacity,
                    Location = r.Location,
                    Type = r.Type.ToString(),
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "name_asc",
                data = results
            });
        }

        /// GET /api/rooms/rooms/location/Cape Town?page=1&pageSize=10
        [HttpGet("rooms/location/{location}")]
        [Authorize(Roles = "Admin,Receptionist,Employee,Facility Manager")]
        public async Task<IActionResult> GetRoomsByLocation(
            string location,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.ConferenceRooms
                .Where(r => r.IsActive && r.Location.Contains(location))
                .AsNoTracking();
    
            query = query.OrderBy(r => r.Name);
       
            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoomListDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Capacity = r.Capacity,
                    Location = r.Location,
                    Type = r.Type.ToString(),
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                data = results
            });
        }

        /// GET /api/rooms/rooms/type/Boardroom?page=1&pageSize=10&sortBy=name
        [HttpGet("rooms/type/{type}")]
        [Authorize(Roles = "Admin,Receptionist,Employee,Facility Manager")]
        public async Task<IActionResult> GetRoomsByType(
            string type,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            if (!Enum.TryParse<RoomType>(type, true, out var roomType))
            {
                return BadRequest($"Invalid room type: {type}. Valid values: Standard, Boardroom, Training");
            }
            
            var query = _context.ConferenceRooms
                .Where(r => r.IsActive && r.Type == roomType)
                .AsNoTracking();

            if (sortBy == "name")
            {
                query = query.OrderBy(r => r.Name);
            }
            else
            {
                query = query.OrderByDescending(r => r.Name);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoomListDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Capacity = r.Capacity,
                    Location = r.Location,
                    Type = r.Type.ToString(),
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "IsActive",
                data = results
            });
        }

        /// GET /api/rooms/rooms/active?page=1&pageSize=10&showActive=true
        [HttpGet("rooms/active")]
        [Authorize(Roles = "Admin,Receptionist,Employee,Facility Manager")]
        public async Task<IActionResult> GetActiveRooms(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? showActive = null)
        {
            var query = _context.ConferenceRooms
                .AsNoTracking();

            if (showActive.HasValue)
            {
                if (showActive.Value == true)
                {
                    query = query.Where(r => r.IsActive ==  true);
                }
                else
                {
                    query = query.Where(r => r.IsActive == false);
                }
            }

            query = query.OrderBy(r => r.Name);

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoomListDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Capacity = r.Capacity,
                    Location = r.Location,
                    Type = r.Type.ToString(),
                    IsActive = r.IsActive
                })
                .ToListAsync();
                
            // show user what filter is applied
            var filterApplied = showActive.HasValue 
                ? (showActive.Value ? "active only" : "inactive only") 
                : "all rooms";

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                filterApplied,
                data = results
            });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConferenceRoomBookingSystem.DTOs;
using System.Security.Claims;

namespace ConferenceRoomBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SortingController : ControllerBase
    {
        private readonly BookingsDbContext _context;
        private readonly IUserService _userService;

        public SortingController(BookingsDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        private IQueryable<Booking> GetBaseBookingQuery()
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Receptionist");

            var query = _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.Status != BookingStatus.Cancelled)
                .AsNoTracking();

            if (!isAdmin)
            {
                query = query.Where(b => b.UserId == userId);
            }

            return query;
        }

        private int GetCurrentUserId()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Username not found in token");
            
            return _userService.GetIntegerUserId(username);
        }

        // ============ BOOKING SORTING ENDPOINTS ============
        /// GET /api/sorting/bookings/roomName?page=1&pageSize=10&sortBy=roomName
        [HttpGet("bookings/roomName")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsSortedByRoomName(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var query = GetBaseBookingQuery();

            if (sortBy == "roomName")
            {
                query = query.OrderBy(b => b.Room != null ? b.Room.Name : "");
            }
            else
            {
                query = query.OrderByDescending(b => b.Room != null ? b.Room.Name : ""); // Default sort
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "roomName_desc",
                data = results
            });
        }

        /// GET /api/sorting/bookings/startTime?page=1&pageSize=10&sortBy=startTime
        [HttpGet("bookings/startTime")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsSortedByStartTime(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var query = GetBaseBookingQuery();

            if (sortBy == "startTime")
            {
                query = query.OrderBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.StartTime);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "startTime_desc",
                data = results
            });
        }

        /// GET /api/sorting/bookings/status/Cancelled?page=1&pageSize=10&sortBy
        [HttpGet("bookings/status/Cancelled")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetCancelledBookings(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = GetBaseBookingQuery()
                .Where(b => b.Status == BookingStatus.Cancelled);
    
            query = query.OrderBy(b => b.CancelledAt ?? b.StartTime);
            

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
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

        /// GET /api/sorting/bookings/createdAt?page=1&pageSize=10&sortBy=createdAt
        [HttpGet("bookings/createdAt")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsSortedByCreatedDate(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var query = GetBaseBookingQuery();

            if (sortBy == "createdAt")
            {
                query = query.OrderBy(b => b.CreatedAt ?? b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.CreatedAt ?? b.StartTime);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "createdAt_desc",
                data = results
            });
        }

        /// GET /api/sorting/bookings/capacity?page=1&pageSize=10&sortBy=capacity
        [HttpGet("bookings/capacity")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsSortedByCapacity(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var query = GetBaseBookingQuery();

            if (sortBy == "capacity")
            {
                query = query.OrderBy(b => b.Room != null ? b.Room.Capacity : 0);
            }
            else
            {
                query = query.OrderByDescending(b => b.Room != null ? b.Room.Capacity : 0);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "startTime_desc",
                data = results
            });
        }

        // ============ BOOKING FILTERING ENDPOINTS ============
        /// GET /api/sorting/bookings/location/Bloemfontein?page=1&pageSize=10&sortBy=startTime
        [HttpGet("bookings/location/{location}")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsByLocation(
            string location,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var query = GetBaseBookingQuery()
                .Where(b => b.Room != null && b.Room.Location.Contains(location));

            if (sortBy == "startTime")
            {
                query = query.OrderBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.StartTime);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "startTime_desc",
                data = results
            });
        }

        /// GET /api/sorting/bookings/room/5?page=1&pageSize=10&sortBy=startTime
        [HttpGet("bookings/room/{roomId}")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsByRoomId(
            int roomId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var query = GetBaseBookingQuery()
                .Where(b => b.RoomId == roomId);

            if (sortBy == "startTime")
            {
                query = query.OrderBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.StartTime);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "startTime_desc",
                data = results
            });
        }

        /// GET /api/sorting/bookings/date/2026/02/15?page=1&pageSize=10&sortBy=startTime
        [HttpGet("bookings/date/{year}/{month}/{day}")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsByDate(
            int year, int month, int day,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var date = new DateTime(year, month, day);
            var nextDay = date.AddDays(1);
            
            var query = GetBaseBookingQuery()
                .Where(b => b.StartTime >= date && b.StartTime < nextDay);

            if (sortBy == "startTime")
            {
                query = query.OrderBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.StartTime);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "startTime_desc",
                data = results
            });
        }

        /// GET /api/sorting/bookings/date-range/2026-02-01/2026-02-28?page=1&pageSize=10&sortBy=startTime
        [HttpGet("bookings/date-range/{from}/{to}")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsByDateRange(
            DateTime from, DateTime to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var toDateEnd = to.Date.AddDays(1).AddTicks(-1);
            
            var query = GetBaseBookingQuery()
                .Where(b => b.StartTime >= from && b.EndTime <= toDateEnd);

            // DEMO 6 — Sorting
            if (sortBy == "startTime")
            {
                query = query.OrderBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.StartTime);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "startTime_desc",
                data = results
            });
        }

        /// GET /api/sorting/bookings/status/Confirmed?page=1&pageSize=10&sortBy=startTime
        [HttpGet("bookings/status/{status}")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsByStatus(
            string status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            if (!Enum.TryParse<BookingStatus>(status, true, out var statusEnum))
            {
                return BadRequest($"Invalid status: {status}. Valid values: Booked, Confirmed, Cancelled, InProgress");
            }
            
            var query = GetBaseBookingQuery()
                .Where(b => b.Status == statusEnum);

            if (sortBy == "startTime")
            {
                query = query.OrderBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.StartTime);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "startTime_desc",
                data = results
            });
        }

        /// GET /api/sorting/bookings/user/2?page=1&pageSize=10&sortBy=startTime
        [HttpGet("bookings/user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBookingsByUserId(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var query = _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserId == userId && b.Status != BookingStatus.Cancelled)
                .AsNoTracking();

            if (sortBy == "startTime")
            {
                query = query.OrderBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.StartTime);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "startTime_desc",
                data = results
            });
        }

        /// GET /api/sorting/bookings/roomType/Standard?page=1&pageSize=10&sortBy=startTime
        [HttpGet("bookings/roomType/{roomType}")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsByRoomType(
            string roomType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            if (!Enum.TryParse<RoomType>(roomType, true, out var typeEnum))
            {
                return BadRequest($"Invalid room type: {roomType}. Valid values: Standard, Boardroom, Training");
            }
            
            var query = GetBaseBookingQuery()
                .Where(b => b.Room != null && b.Room.Type == typeEnum);

            if (sortBy == "startTime")
            {
                query = query.OrderBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.StartTime);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "startTime_desc",
                data = results
            });
        }

        // ============ USER-SPECIFIC ENDPOINT ============

        /// GET /api/sorting/my-bookings?page=1&pageSize=10&sortBy=startTime
        [HttpGet("my-bookings")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetMyBookings(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var userId = GetCurrentUserId();
            
            var query = _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserId == userId && b.Status != BookingStatus.Cancelled)
                .AsNoTracking();

            if (sortBy == "startTime")
            {
                query = query.OrderBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.StartTime);
            }

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "startTime_desc",
                data = results
            });
        }

        // ============ ROOM ENDPOINTS ============

        /// GET /api/sorting/rooms/name?page=1&pageSize=10
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

        /// GET /api/sorting/rooms/capacity?page=1&pageSize=10&sortBy=capacity
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

        /// GET /api/sorting/rooms/location/Cape Town?page=1&pageSize=10
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

        /// GET /api/sorting/rooms/type/Boardroom?page=1&pageSize=10&sortBy=name
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

            // DEMO 6 — Sorting
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
                sortBy = sortBy ?? "name_asc",
                data = results
            });
        }

        // ============ PRIVATE HELPER METHODS ============

        
    }
}
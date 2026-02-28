using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ConferenceRoomBookingSystem;
using ConferenceRoomBookingSystem.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IUserService _userService;
        private readonly BookingsDbContext _context;

        public BookingController(IBookingRepository bookingRepository, IRoomRepository roomRepository, IUserService userService, BookingsDbContext context)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _userService = userService;
            _context = context;
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
            {
                throw new UnauthorizedAccessException("Username not found in token");
            }
            
            return _userService.GetIntegerUserId(username);
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Receptionist")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto bookingDto)
        {
            var room = await _roomRepository.GetByIdAsync(bookingDto.Room.Id);
            if (room == null)
            {
                return NotFound(new { error = "Room not found" });
            }

            // Validate that end time is after start time
            if (bookingDto.StartTime >= bookingDto.EndTime)
            {
                return BadRequest(new { error = "End time must be after start time" });
            }

            bool overlaps = await _bookingRepository.HasOverlapAsync(
                room.Id, 
                bookingDto.BookingDate,
                bookingDto.StartTime, 
                bookingDto.EndTime);

            if (overlaps)
            {
                return Conflict(new { error = "Booking overlaps with an existing booking" });
            }

            var userId = GetCurrentUserId();
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            var booking = new Booking(room, userId, username, 
                                     bookingDto.BookingDate, 
                                     bookingDto.StartTime, 
                                     bookingDto.EndTime);

            booking.ConfirmBooking();
    
            var createdBooking = await _bookingRepository.AddAsync(booking);

            var response = new BookingResponseDto
            {
                Id = createdBooking.Id,
                Room = new RoomDto
                {
                    Id = createdBooking.Room.Id,
                    Name = createdBooking.Room.Name,
                    Capacity = createdBooking.Room.Capacity,
                    Type = createdBooking.Room.Type.ToString(),
                    Location = createdBooking.Room.Location,
                    IsActive = createdBooking.Room.IsActive
                },
                BookingDate = createdBooking.BookingDate,
                StartTime = createdBooking.StartTime,
                EndTime = createdBooking.EndTime,
                Status = createdBooking.Status.ToString(),
                UserId = createdBooking.UserId,
                BookedBy = createdBooking.BookedBy,
                Capacity = createdBooking.Capacity,
                CreatedAt = createdBooking.CreatedAt,
                CancelledAt = createdBooking.CancelledAt
            };

            return Ok(response);
        }

        [HttpGet("maintenance")]
        [Authorize(Roles = "Facility Manager")]
        public IActionResult GetMaintenanceInfo()
        {
            return Ok("Accessing maintenance booking");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if(booking == null)
            {
                return NotFound(new
                {
                    error = "Booking not found or cannot be cancelled",
                    detail = $"Booking with id {id} not found or cannot be cancelled"
                });
            }

            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && booking.UserId != userId)
            {
                return Forbid();
            }

            booking.CancelBooking();
            await _bookingRepository.UpdateAsync(booking);

            return Ok(new
            {
                message = "Booking cancelled successfully",
                bookingId = id,
                cancelledAt = booking.CancelledAt
            });     
        }

        // ============ BOOKING SORTING ENDPOINTS ============

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
                query = query.OrderByDescending(b => b.Room != null ? b.Room.Name : "");
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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
                query = query.OrderBy(b => b.BookingDate).ThenBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime);
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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

        [HttpGet("bookings/status/Cancelled")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetCancelledBookings(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = GetBaseBookingQuery()
                .Where(b => b.Status == BookingStatus.Cancelled);
    
            query = query.OrderBy(b => b.CancelledAt ?? b.BookingDate.ToDateTime(b.StartTime));

            var totalCount = await query.CountAsync();

            var results = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListDto
                {
                    Id = b.Id,
                    RoomName = b.Room != null ? b.Room.Name : "Unknown",
                    Location = b.Room != null ? b.Room.Location : "Unknown",
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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
                query = query.OrderBy(b => b.CreatedAt ?? b.BookingDate.ToDateTime(b.StartTime));
            }
            else
            {
                query = query.OrderByDescending(b => b.CreatedAt ?? b.BookingDate.ToDateTime(b.StartTime));
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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
                query = query.OrderBy(b => b.Room != null ? b.Room.Capacity : 0)
                            .ThenBy(b => b.BookingDate)
                            .ThenBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.Room != null ? b.Room.Capacity : 0)
                            .ThenByDescending(b => b.BookingDate)
                            .ThenByDescending(b => b.StartTime);
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
                })
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                sortBy = sortBy ?? "capacity_desc",
                data = results
            });
        }

        // ============ BOOKING FILTERING ENDPOINTS ============

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
                query = query.OrderBy(b => b.BookingDate).ThenBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime);
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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
                query = query.OrderBy(b => b.BookingDate).ThenBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime);
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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

        [HttpGet("bookings/date/{year}/{month}/{day}")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsByDate(
            int year, int month, int day,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var date = new DateOnly(year, month, day);
            
            var query = GetBaseBookingQuery()
                .Where(b => b.BookingDate == date);

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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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

        [HttpGet("bookings/date-range/{from}/{to}")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetBookingsByDateRange(
            DateOnly from, DateOnly to,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null)
        {
            var query = GetBaseBookingQuery()
                .Where(b => b.BookingDate >= from && b.BookingDate <= to);

            if (sortBy == "startTime")
            {
                query = query.OrderBy(b => b.BookingDate).ThenBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime);
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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
                query = query.OrderBy(b => b.BookingDate).ThenBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime);
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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
                query = query.OrderBy(b => b.BookingDate).ThenBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime);
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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
                query = query.OrderBy(b => b.BookingDate).ThenBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime);
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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
                query = query.OrderBy(b => b.BookingDate).ThenBy(b => b.StartTime);
            }
            else
            {
                query = query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.StartTime);
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
                    BookingDate = b.BookingDate,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status.ToString(),
                    BookedBy = b.BookedBy
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
    }
}
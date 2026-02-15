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
        //private readonly BookingManager _bookingManager;
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

        [HttpGet]
        [Authorize(Roles = "Admin,Employee,Receptionist")]
        public async Task<IActionResult> GetAllBookings()
        {
            var userId = GetCurrentUserId();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Receptionist");


            IEnumerable<Booking> bookings;

            if (isAdmin)
            {
                bookings = await _bookingRepository.GetAllAsync();
            }
            else
            {
                bookings = await _bookingRepository.GetByUserIdAsync(userId);
            }

            //var userBookings = bookings.Where(b => b.UserId == userId).ToList();

            var response = bookings.Select(b => new BookingResponseDto
            {
                Id = b.Id,
                Room = new RoomDto
                {
                    Id = b.Room.Id,
                    Name = b.Room.Name,
                    Capacity = b.Room.Capacity,
                    Type = b.Room.Type.ToString(),
                    Location = b.Room.Location,
                    IsActive = b.Room.IsActive
                },
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString(),
                UserId = b.UserId,
                Capacity = b.Capacity,
                CreatedAt = b.CreatedAt,
                CancelledAt = b.CancelledAt
            }).ToList();
    
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Employee,Receptionist")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new
                {
                    error = "Booking not found",
                    detail = $"Booking with id {id} not found"
                });
            }

            //Check if the user is authorized to view this booking
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Receptionist");

            if (!isAdmin && booking.UserId != userId)
            {
                return Forbid();
            }

            var response = new BookingResponseDto
            {
                Id = booking.Id,
                Room = new RoomDto
                {
                    Id = booking.Room.Id,
                    Name = booking.Room.Name,
                    Capacity = booking.Room.Capacity,
                    Type = booking.Room.Type.ToString(),
                    Location = booking.Room.Location,
                    IsActive = booking.Room.IsActive
                },
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Status = booking.Status.ToString(),
                UserId = booking.UserId,
                Capacity = booking.Capacity,
                CreatedAt = booking.CreatedAt,
                CancelledAt = booking.CancelledAt
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Receptionist")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto bookingDto)
        {

            var room = await _roomRepository.GetByIdAsync(bookingDto.Room.Id);

            bool overlaps = await _bookingRepository.HasOverlapAsync(
            room.Id, 
            bookingDto.StartTime, 
            bookingDto.EndTime);

            var userId = GetCurrentUserId();

            var booking = new Booking(room, userId, bookingDto.StartTime, bookingDto.EndTime);

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
                StartTime = createdBooking.StartTime,
                EndTime = createdBooking.EndTime,
                Status = createdBooking.Status.ToString(),
                UserId = createdBooking.UserId,
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

        // private int GetCurrentUserId()
        // {
        //     var username = User.FindFirst(ClaimTypes.Name)?.Value;
        //     if (string.IsNullOrEmpty(username))
        //     {
        //         throw new UnauthorizedAccessException("Username not found in token");
        //     }
        //     int userId = _userService.GetIntegerUserId(username);

        //     if(userId == 0)
        //     {
        //         throw new UnauthorizedAccessException($"User '{username}' not found in system");
        //     }
        //     return userId;
        // }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            //Get the booking by id
            var booking = await _bookingRepository.GetByIdAsync(id);
            if(booking == null)
            {
                return NotFound(new
                {
                    error = "Booking not found or cannot be cancelled",
                    detail = $"Booking with id {id} not found or cannot be cancelled"
                });
            }

            //Authorize only the user who created the booking or an admin to cancel
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && booking.UserId != userId)
            {
                return Forbid();
            }

            //Cancel the booking
            booking.CancelBooking();
            await _bookingRepository.UpdateAsync(booking);

            return Ok(new
            {
                message = "Booking cancelled successfully",
                bookingId = id,
                cancelledAt = booking.CancelledAt
            });     
        }

        [HttpGet("rooms")]
        [Authorize(Roles = "Admin,Receptionist,Employee,Facility Manager")]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomRepository.GetAllAsync();
            var response = rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                Type = r.Type.ToString(),
                Location = r.Location,
                IsActive = r.IsActive
            }).ToList();
    
            return Ok(response);
        }

        [HttpGet("rooms/active")]
        [Authorize(Roles = "Admin,Receptionist,Employee")]
        public async Task<IActionResult> GetActiveRooms()
        {
            var rooms = await _roomRepository.GetActiveRoomsAsync();
            var response = rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                Type = r.Type.ToString(),
                Location = r.Location,
                IsActive = r.IsActive
            }).ToList();

            return Ok(response);
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

        /// <summary>
        /// GET /api/sorting/rooms/active?page=1&pageSize=10
        /// Get only active rooms (IsActive = true)
        /// </summary>
        [HttpGet("rooms/Isactive")]
        [Authorize(Roles = "Admin,Receptionist,Employee,Facility Manager")]
        public async Task<IActionResult> GetActiveRooms(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.ConferenceRooms
                .Where(r => r.IsActive == true)  // Filter for active rooms only
                .OrderBy(r => r.Name)            // Default sort by name
                .AsNoTracking();

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

        /// <summary>
        /// GET /api/sorting/rooms/active?page=1&pageSize=10
        /// Get only inactive rooms (IsActive = false)
        /// </summary>
        [HttpGet("rooms/isinactive")]
        [Authorize(Roles = "Admin,Receptionist,Employee,Facility Manager")]
        public async Task<IActionResult> GetInActiveRooms(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.ConferenceRooms
                .Where(r => r.IsActive == false)  // Filter for inactive rooms only
                .OrderBy(r => r.Name)            // Default sort by name
                .AsNoTracking();

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

    }
}
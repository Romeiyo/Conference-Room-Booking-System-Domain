using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ConferenceRoomBookingSystem;

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

        public BookingController(IBookingRepository bookingRepository, IRoomRepository roomRepository, IUserService userService)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _userService = userService;
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
                    Type = b.Room.Type.ToString()
                },
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status.ToString(),
                UserId = b.UserId
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
                    Type = booking.Room.Type.ToString()
                },
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Status = booking.Status.ToString(),
                UserId = booking.UserId
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
                    Type = createdBooking.Room.Type.ToString()
                },                    
                StartTime = createdBooking.StartTime,
                EndTime = createdBooking.EndTime,
                Status = createdBooking.Status.ToString(),
                UserId = createdBooking.UserId
            };

            return Ok(response);
           
        }

        [HttpGet("maintenance")]
        [Authorize(Roles = "Facility Manager")]
        public IActionResult GetMaintenanceInfo()
        {
            return Ok("Accessing maintenance booking");
        }

        private int GetCurrentUserId()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                throw new UnauthorizedAccessException("Username not found in token");
            }
            int userId = _userService.GetIntegerUserId(username);

            if(userId == 0)
            {
                throw new UnauthorizedAccessException($"User '{username}' not found in system");
            }
            return userId;
        }

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
                bookingId = id
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
                Type = r.Type.ToString()
            }).ToList();
    
            return Ok(response);
        }
    }
}
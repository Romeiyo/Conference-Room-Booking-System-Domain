using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBookingSystem;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly BookingManager _bookingManager;
        private readonly RoomRepository _roomRepository;
        private readonly IUserService _userService;

        public BookingController(BookingManager bookingManager, RoomRepository roomRepository, IUserService userService)
        {
            _bookingManager = bookingManager;
            _roomRepository = roomRepository;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Employee,Receptionist")]
        public async Task<IActionResult> GetAllBookings()
        {
            var userId = GetCurrentUserId();

            var isAdmin = User.IsInRole("Admin");
            var bookings = _bookingManager.GetBookings();

            var filteredBookings = isAdmin ? bookings : bookings.Where(b => b.UserId == userId).ToList();

            //var userBookings = bookings.Where(b => b.UserId == userId).ToList();

            var response = filteredBookings.Select(b => new BookingResponseDto
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
            var booking = _bookingManager.GetBookingById(id);

            if (booking == null)
            {
                return NotFound(new
                {
                    error = "Booking not found",
                    detail = $"Booking with id {id} not found"
                });
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
                Status = booking.Status.ToString()
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Receptionist")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto bookingDto)
        {
            // if (bookingDto?.Room == null)
            // {
            //     return BadRequest("Room is required");
            // }

            // //Validate the room exists in our repository
            // if (!_roomRepository.RoomExists(bookingDto.Room))
            // {
            //     return BadRequest($"Room not found or invalid");
            // }

            var existingRoom = _roomRepository.GetRoomById(bookingDto.Room.Id);
            
            var userId = GetCurrentUserId();

            var request = new BookingRequest(existingRoom, userId, bookingDto.StartTime, bookingDto.EndTime);
            
            var createdBooking = await _bookingManager.CreateBookingAsync(request);
    
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
            
            
            // catch (ArgumentException ex)
            // {
            //     return BadRequest(ex.Message);
            // }
            // catch (BookingConflictException ex)
            // {
            //     return Conflict(ex.Message);
            // }
            // catch (Exception ex)
            // {
            //     return StatusCode(500, $"An error occurred while creating the booking: {ex.Message}");
            // }    
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
            var success = await _bookingManager.CancelBookingAsync(id);

            if(!success)
            {
                return NotFound(new
                {
                    error = "Booking not found or cannot be cancelled",
                    detail = $"Booking with id {id} not found or cannot be cancelled"
                });
            }

            return Ok(new
            {
                message = "Booking cancelled successfully",
                bookingId = id
            });     
        }
    }
}
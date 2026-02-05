using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBookingSystem;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingManager _bookingManager;
        private readonly RoomRepository _roomRepository;

        public BookingController(BookingManager bookingManager, RoomRepository roomRepository)
        {
            _bookingManager = bookingManager;
            _roomRepository = roomRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = _bookingManager.GetBookings();

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
                Status = b.Status.ToString()
            }).ToList();
    
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto bookingDto)
        {
            if (bookingDto?.Room == null)
            {
                return BadRequest("Room is required");
            }

            //Validate the room exists in our repository
            if (!_roomRepository.RoomExists(bookingDto.Room))
            {
                return BadRequest($"Room not found or invalid");
            }

            // Get the exact room reference from repository
            var existingRoom = _roomRepository.GetRoomById(bookingDto.Room.Id);
            var request = new BookingRequest(existingRoom, bookingDto.StartTime, bookingDto.EndTime);
            try
            {
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
                    Status = createdBooking.Status.ToString()
                };

                return Ok(response); // Changed from return Ok(createdBooking)
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (BookingConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the booking: {ex.Message}");
            }

            
        }

    }
}
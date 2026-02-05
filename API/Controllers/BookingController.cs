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
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto bookingDto)
        {
            // var createdBooking = _bookingManager.CreateBooking(bookingRequest);
            // return Ok(createdBooking);
            if (bookingDto?.Room == null)
            {
                return BadRequest("Room is required");
            }

            //Validate the room exists in our repository
            if (!_roomRepository.RoomExists(bookingDto.Room))
            {
                return BadRequest($"Room not found or invalid");
            }

            // var existingRoom = _roomRepository.GetRoomById(bookingDto.Room.Id);
            // if (existingRoom == null)
            // {
            //     return BadRequest($"Room with ID {bookingDto.Room.Id} not found");
            // }

            

            // Get the exact room reference from repository
            var existingRoom = _roomRepository.GetRoomById(bookingDto.Room.Id);
            var request = new BookingRequest(existingRoom, bookingDto.StartTime, bookingDto.EndTime);
            try
            {
                var createdBooking = await _bookingManager.CreateBookingAsync(request);
                return Ok(createdBooking);
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
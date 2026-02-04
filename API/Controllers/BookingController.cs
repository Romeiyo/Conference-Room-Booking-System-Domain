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

        public BookingController(BookingManager bookingManager)
        {
            _bookingManager = bookingManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = _bookingManager.GetBookings();
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest bookingRequest)
        {
            var createdBooking = _bookingManager.CreateBooking(bookingRequest);
            return Ok(createdBooking);
        }

    }
}
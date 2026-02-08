using Microsoft.AspNetCore.Mvc;
using ConferenceRoomBookingSystem;
using Microsoft.AspNetCore.Authorization;

namespace ConferenceRoomBookingSystem
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var token = await _userService.AuthenticateAsync(
                loginModel.Username, 
                loginModel.Password);

            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new 
            { 
                token,
                username = loginModel.Username,
                expires = DateTime.UtcNow.AddHours(2)
            });
        }

        [HttpGet("test")]
        [Authorize]
        public IActionResult TestAuth()
        {
            return Ok(new { message = "You are authenticated!" });
        }
    }
}
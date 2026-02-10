using Microsoft.AspNetCore.Identity;

namespace ConferenceRoomBookingSystem
{
    public interface IUserService
    {
        Task<string> AuthenticateAsync(string username, string password);
        Task<ApplicationUser> GetUserByUsernameAsync(string username);
        int GetIntegerUserId(string username);
    }

    public class UserService : IUserService
    {
        public readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;

        public UserService(UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return null;

            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            return _jwtService.GenerateToken(user.UserName, role, user.Id);
        }

        public async Task<ApplicationUser> GetUserByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public int GetIntegerUserId(string username)
        {
            return username switch
            {
                "Admin" => 1,
                "Employee1" => 2,
                "Employee2" => 3,
                "Facilitator" => 4,
                "Receptionist" => 5,
                _ => 0
            };
        }
    }
}

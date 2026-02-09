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
        // private readonly List<UserModel> _users = new()
        // {
        //     new UserModel { Id = 1, Username = "Admin", Password = "admin123", Role = "Admin" },
        //     new UserModel { Id = 2, Username = "Employee1", Password = "password1", Role = "Employee" },
        //     new UserModel { Id = 3, Username = "Employee2", Password = "password2", Role = "Employee" },
        //     new UserModel { Id = 4, Username = "Facilitator", Password = "password3", Role = "Facility Manager" },
        //     new UserModel { Id = 5, Username = "Receptionist", Password = "password4", Role = "Receptionist" },
        // };

        

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

// UserService.cs - REPLACE entire file with this
// using Microsoft.AspNetCore.Identity;
// using System.Threading.Tasks;
// using System.Linq;

// namespace ConferenceRoomBookingSystem
// {
//     public class UserService : IUserService
//     {
//         private readonly UserManager<ApplicationUser> _userManager;
//         private readonly IJwtService _jwtService;
//         private readonly RoleManager<IdentityRole> _roleManager;

//         public UserService(
//             UserManager<ApplicationUser> userManager,
//             IJwtService jwtService,
//             RoleManager<IdentityRole> roleManager)
//         {
//             _userManager = userManager;
//             _jwtService = jwtService;
//             _roleManager = roleManager;
//         }

//         public async Task<string> AuthenticateAsync(string username, string password)
//         {
//             var user = await _userManager.FindByNameAsync(username);
//             if (user == null) return null;

//             // Identity handles password verification automatically
//             var passwordValid = await _userManager.CheckPasswordAsync(user, password);
//             if (!passwordValid) return null;

//             // Get user's role
//             var roles = await _userManager.GetRolesAsync(user);
//             var role = roles.FirstOrDefault() ?? "User";

//             // Generate token with username and role
//             return _jwtService.GenerateToken(user.UserName, role);
//         }

//         public async Task<ApplicationUser> GetUserByUsername(string username)
//         {
//             return await _userManager.FindByNameAsync(username);
//         }

//         public async Task<bool> UserExists(string username)
//         {
//             return await _userManager.FindByNameAsync(username) != null;
//         }

//         // Helper method to map Identity User ID to your integer user ID
//         public int GetIntegerUserId(string username)
//         {
//             return username switch
//             {
//                 "Admin" => 1,
//                 "Employee1" => 2,
//                 "Employee2" => 3,
//                 "Facilitator" => 4,
//                 "Receptionist" => 5,
//                 _ => 0
//             };
//         }
//     }
// }
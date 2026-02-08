using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using API.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ConferenceRoomBookingSystem;

namespace ConferenceRoomBookingSystem
{
    public interface IUserService
    {
        Task<string> AuthenticateAsync(string username, string password);
        Task<IdentityUser> GetUserByIdAsync(string userId);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;

        public UserService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtService = jwtService;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            // Find user by username or email
            var user = await _userManager.FindByNameAsync(username) 
                      ?? await _userManager.FindByEmailAsync(username);
            
            if (user == null)
                return null;
            
            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            
            if (!result.Succeeded)
                return null;
            
            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "User";
            
            // Generate token
            return _jwtService.GenerateToken(user.UserName, primaryRole);
        }

        public async Task<IdentityUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}
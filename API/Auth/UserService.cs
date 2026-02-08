namespace ConferenceRoomBookingSystem
{
    public interface IUserService
    {
        Task<string> AuthenticateAsync(string username, string password);
    }

    public class UserService : IUserService
    {
        
        private readonly List<UserModel> _users = new()
        {
            new UserModel { Id = 1, Username = "Admin", Password = "admin123", Role = "Admin" },
            new UserModel { Id = 2, Username = "Employee1", Password = "password1", Role = "Employee" },
            new UserModel { Id = 3, Username = "Employee2", Password = "password2", Role = "Employee" },
            new UserModel { Id = 4, Username = "Facilitator", Password = "password3", Role = "Facility Manager" },
            new UserModel { Id = 5, Username = "Receptionist", Password = "password4", Role = "Receptionist" },
        };

        private readonly IJwtService _jwtService;

        public UserService(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = _users.FirstOrDefault(u => 
                u.Username == username && u.Password == password);
            
            if (user == null)
                return null;

            return _jwtService.GenerateToken(user.Username, user.Role);
        }
    }
}
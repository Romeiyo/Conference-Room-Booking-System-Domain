using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConferenceRoomBookingSystem
{
    public interface IDatabaseSeeder
    {
        Task SeedAsync();
    }

    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(IServiceProvider serviceProvider, ILogger<DatabaseSeeder> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            
            var context = services.GetRequiredService<BookingsDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            
            // Apply migrations
            await context.Database.MigrateAsync();
            
            // Seed roles
            await SeedRolesAsync(roleManager);
            
            // Seed users
            await SeedUsersAsync(userManager);

            await SeedRoomsAsync(context);
            
            _logger.LogInformation("Database seeded successfully!");

            await UpdateExistingBookingsAsync(context);

            _logger.LogInformation("Existing bookings updated successfully!");
        }

        private async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Employee", "Facility Manager", "Receptionist", "User" };
            
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation("Created role: {Role}", role);
                }
            }
        }

        private async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUsers = new[]
            {
                new { Username = "Admin", Password = "Admin123!", Email = "admin@example.com", Role = "Admin" },
                new { Username = "Employee1", Password = "Password1!", Email = "emp1@example.com", Role = "Employee" },
                new { Username = "Employee2", Password = "Password2!", Email = "emp2@example.com", Role = "Employee" },
                new { Username = "Facilitator", Password = "Password3!", Email = "facility@example.com", Role = "Facility Manager" },
                new { Username = "Receptionist", Password = "Password4!", Email = "reception@example.com", Role = "Receptionist" },
            };

            _logger.LogInformation("Starting user seeding...");

            foreach (var userInfo in defaultUsers)
            {
                _logger.LogInformation("Checking user: {Username}", userInfo.Username);

                var existingUser = await userManager.FindByNameAsync(userInfo.Username);

                if (existingUser == null)
                {
                    _logger.LogInformation("Creating user: {Username}", userInfo.Username);

                    var user = new ApplicationUser
                    {
                        UserName = userInfo.Username,
                        Email = userInfo.Email,
                        EmailConfirmed = true
                    };
                    
                    var result = await userManager.CreateAsync(user, userInfo.Password);
                    
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, userInfo.Role);
                        _logger.LogInformation("Created user: {Username}", userInfo.Username);
                    }
                    else
                    {
                        _logger.LogError("✗ Failed to create user {Username}: {Errors}", 
                    userInfo.Username, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    _logger.LogInformation("User {Username} already exists", userInfo.Username);
                }
            }

            _logger.LogInformation("User seeding completed");
        }

        private async Task SeedRoomsAsync(BookingsDbContext context)
        {
            if (!await context.ConferenceRooms.AnyAsync())
            {
                var seedData = new SeedData();
                var rooms = seedData.SeedRooms();
                
                await context.ConferenceRooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();
                
                _logger.LogInformation("Seeded {Count} rooms to database", rooms.Count);
            }
        }

        private async Task UpdateExistingBookingsAsync(BookingsDbContext context)
        {
            var bookingsWithoutCapacity = await context.Bookings
                .Where(b => b.Capacity == 0)
                .Include(b => b.Room)
                .ToListAsync();
    
            foreach (var booking in bookingsWithoutCapacity)
            {
                // Set capacity from room
                booking.Capacity = booking.Room?.Capacity ?? 10;
        
                // Set CreatedAt if null (for existing records)
                if (booking.CreatedAt == DateTime.MinValue)
                {
                    booking.CreatedAt = DateTime.UtcNow.AddDays(-30); // Default to 30 days ago
                }
            }
    
            if (bookingsWithoutCapacity.Any())
            {
                await context.SaveChangesAsync();
                _logger.LogInformation("Updated {Count} existing bookings with new columns", 
                    bookingsWithoutCapacity.Count);
            }
        }
    }
}
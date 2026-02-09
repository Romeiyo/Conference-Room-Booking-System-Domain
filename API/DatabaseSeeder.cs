// Add this file: DatabaseSeeder.cs (place in root or create Services/ folder)
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
            
            _logger.LogInformation("Database seeded successfully!");
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
    }
}
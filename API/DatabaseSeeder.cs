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

            await SeedBookingsAsync(context);

            _logger.LogInformation("Existing bookings seeded successfully!");
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
            var seedData = new SeedData();
            var rooms = seedData.SeedRooms();
            
            var existingRoomIds = await context.ConferenceRooms
                .Select(r => r.Id)
                .ToListAsync();

            // Find rooms that don't exist in the database
            var newRooms = rooms
                .Where(r => !existingRoomIds.Contains(r.Id))
                .ToList();

            if(newRooms.Any())
            {
                await context.ConferenceRooms.AddRangeAsync(newRooms);
                await context.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation("No new rooms to seed. All rooms already exist in the database.");
            }
            
            _logger.LogInformation("Seeded {Count} rooms to database", newRooms.Count);
            
        }

        // private async Task SeedBookingsAsync(BookingsDbContext context)
        // {
        //     var rooms = await context.ConferenceRooms.ToListAsync();

        //     if (!rooms.Any())
        //     {
        //         _logger.LogWarning("No rooms found in database. Skipping booking seeding.");
        //         await SeedRoomsAsync(context);
        //         rooms = await context.ConferenceRooms.ToListAsync();
        //     }

        //     var seedData = new SeedData();
        //     var bookings = seedData.SeedBookings();

        //     int newBookingsCount = 0;
        //     int updatedBookingsCount = 0;

        //     foreach (var seedBooking in bookings)
        //     {
        //         var existingBooking = await context.Bookings.FindAsync(seedBooking.Id);

        //         if (existingBooking == null)
        //         {
        //             var room = rooms.FirstOrDefault(r => r.Id == seedBooking.RoomId);
        //             if (room == null)
        //             {
        //                 _logger.LogWarning("Room with ID {RoomId} not found for booking {BookingId}. Skipping this booking.", 
        //                     seedBooking.RoomId, seedBooking.Id);
        //                 continue;
        //             }

        //             context.Bookings.Add(seedBooking);
        //             newBookingsCount++;

        //             _logger.LogInformation("Added new booking with ID {BookingId} for room {RoomName}", 
        //                 seedBooking.Id, room.Name);
        //         }
        //         else
        //         {
        //             existingBooking.StartTime = seedBooking.StartTime;
        //             existingBooking.EndTime = seedBooking.EndTime;
        //             existingBooking.UserId = seedBooking.UserId;
        //             existingBooking.RoomId = seedBooking.RoomId;
        //             existingBooking.Status = seedBooking.Status;
        //             existingBooking.Capacity = seedBooking.Capacity;
        //             existingBooking.CreatedAt = seedBooking.CreatedAt;
        //             existingBooking.CancelledAt = seedBooking.CancelledAt;

        //             updatedBookingsCount++;

        //             _logger.LogInformation("Updated existing booking with ID {BookingId}", 
        //                 existingBooking.Id);
        //         }
        //     }

        //     if (newBookingsCount > 0 || updatedBookingsCount > 0)
        //     {
        //         await context.SaveChangesAsync();
        //         _logger.LogInformation("Saved {NewCount} new bookings and updated {UpdatedCount} existing bookings to database", 
        //             newBookingsCount, updatedBookingsCount);
        //     }
        //     else
        //     {
        //         _logger.LogInformation("No new bookings to add or update. All bookings are up to date.");
        //     }
        // }
        private async Task SeedBookingsAsync(BookingsDbContext context)
        {
            // Get rooms from database (already tracked)
            var rooms = await context.ConferenceRooms.ToListAsync();
            
            var seedData = new SeedData();
            var bookings = seedData.SeedBookings();
            
            // Clear the Room navigation property to avoid tracking issues
            foreach (var booking in bookings)
            {
                booking.Room = null; // Detach the room object
            }
            
            // Get existing booking IDs
            var existingBookingIds = await context.Bookings
                .Select(b => b.Id)
                .ToListAsync();
            
            var newBookings = bookings
                .Where(b => !existingBookingIds.Contains(b.Id))
                .ToList();
            
            if (newBookings.Any())
            {
                // Re-attach correct room references
                foreach (var booking in newBookings)
                {
                    var room = rooms.FirstOrDefault(r => r.Id == booking.RoomId);
                    if (room != null)
                    {
                        booking.Room = room;
                        booking.Capacity = room.Capacity; // Ensure capacity matches room
                    }
                }
                
                await context.Bookings.AddRangeAsync(newBookings);
                await context.SaveChangesAsync();
                
                _logger.LogInformation("Added {Count} new bookings to database", newBookings.Count);
            }
            else
            {
                _logger.LogInformation("No new bookings to add");
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
using ConferenceRoomBookingSystem;
using ConferenceRoomBookingSystem.Persistence;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT secret key is not configured");

//Database configuration
builder.Services.AddDbContext<BookingsDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<BookingsDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true, 
        ValidateAudience = true, 
        ValidateLifetime = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
    };
});

//Services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add services to the container.
var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
var bookingsFilePath = Path.Combine(dataDirectory, "bookings.json");
Directory.CreateDirectory(dataDirectory);
builder.Services.AddScoped<BookingManager>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

//Services for authentication
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// builder.Services.AddAuthorization();

// builder.Services.AddSingleton<IJwtService, JwtService>();
// builder.Services.AddSingleton<IUserService, UserService>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var seeder = services.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAsync();

        var context = services.GetRequiredService<BookingsDbContext>();
        await SeedRoomsAsync(context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error seeding database");
    }
}

app.Run();

async Task SeedRoomsAsync(BookingsDbContext context, ILogger logger)
{
    if (!context.ConferenceRooms.Any())
    {
        var seedData = new SeedData();
        var rooms = seedData.SeedRooms();
        
        await context.ConferenceRooms.AddRangeAsync(rooms);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Seeded {Count} rooms to database", rooms.Count);
    }
}
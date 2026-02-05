using ConferenceRoomBookingSystem;
using ConferenceRoomBookingSystem.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//var builder = WebApplication.CreateBuilder(args);
var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
var bookingsFilePath = Path.Combine(dataDirectory, "bookings.json");

Directory.CreateDirectory(dataDirectory);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<BookingManager>();
builder.Services.AddSingleton<IBookingStore>(sp => new BookingFileStore(bookingsFilePath));


var app = builder.Build();

app.MapControllers();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();

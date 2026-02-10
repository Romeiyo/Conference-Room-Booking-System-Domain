using ConferenceRoomBookingSystem;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class BookingsDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<ConferenceRoom> ConferenceRooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public BookingsDbContext(DbContextOptions<BookingsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Booking entity
        builder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Id).ValueGeneratedOnAdd();
            
            // Room relationship
            entity.HasOne(b => b.Room)
                  .WithMany()
                  .HasForeignKey(b => b.RoomId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.Property(b => b.Status)
                  .HasConversion<string>(); // Store enum as string
                  
            entity.Property(b => b.StartTime).IsRequired();
            entity.Property(b => b.EndTime).IsRequired();
            entity.Property(b => b.UserId).IsRequired();
            entity.Property(b => b.RoomId).IsRequired();
        });

        // Configure ConferenceRoom entity
        builder.Entity<ConferenceRoom>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).ValueGeneratedOnAdd();
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Capacity).IsRequired();
            entity.Property(r => r.Type).HasConversion<string>();
        });
        }
}
using System;
using System.Collections.Generic;
using System.Linq;
using ConferenceRoomBookingSystem.Enums;
using ConferenceRoomBookingSystem.Models;

public static class BookingFunction
{
    public static void BookConferenceRoom(
        List<ConferenceRoom> availableRooms,
        List<ConferenceRoom> unavailableRooms,
        List<Booking> bookings)
    {
        Console.WriteLine();
        Console.WriteLine("\n=== Book a Conference Room ===");
           
        Console.WriteLine("\nAvailable Rooms:");
        Console.WriteLine("ID\tName\tCapacity");
        Console.WriteLine("----------------------");
        foreach (var room in availableRooms)
        {
            Console.WriteLine($"{room.Id}\t{room.Name}\t{room.Capacity}\t{room.Type}");
        }
            
        Console.Write("\nEnter Room ID to book (or 0 to cancel): ");
        string roomIdInput = Console.ReadLine();
            
        if (!int.TryParse(roomIdInput, out int roomId) || roomId == 0)
        {
            Console.WriteLine("Booking cancelled.");
            return;
        }
            
        var roomToBook = availableRooms.FirstOrDefault(r => r.Id == roomId);
        if (roomToBook == null)
        {
            Console.WriteLine("Invalid Room ID. Please try again.");
            return;
        }
            
        Console.Write("Enter your User ID: ");
        string userIdInput = Console.ReadLine();
            
        if (!int.TryParse(userIdInput, out int userId))
        {
            Console.WriteLine("Invalid User ID. Booking cancelled.");
            return;
        }
            
        Console.Write("Enter start time (yyyy-MM-dd HH:mm): ");
        string startTimeInput = Console.ReadLine();
          
        if (!DateTime.TryParse(startTimeInput, out DateTime startTime))
        {
            Console.WriteLine("Invalid date format. Booking cancelled.");
            return;
        }
            
        Console.Write("Enter end time (yyyy-MM-dd HH:mm): ");
        string endTimeInput = Console.ReadLine();
           
        if (!DateTime.TryParse(endTimeInput, out DateTime endTime))
        {
            Console.WriteLine("Invalid date format. Booking cancelled.");
            return;
        }
            
        if (endTime <= startTime)
        {
            Console.WriteLine("End time must be after start time. Booking cancelled.");
            return;
        }
            
        bool hasOverlap = false;
        foreach (var booking in bookings)
        {
            if (booking.Room.Id == roomId)
            {
                if ((startTime >= booking.StartTime && startTime < booking.EndTime) ||
                    (endTime > booking.StartTime && endTime <= booking.EndTime) ||
                    (startTime <= booking.StartTime && endTime >= booking.EndTime))
                {
                    hasOverlap = true;
                    break;
                }
            }
        }
           
        if (hasOverlap)
        {
            Console.WriteLine("This room is already booked during that time. Please choose another time.");
            return;
        }

        var bookingRecord = new Booking(roomToBook, userId, startTime, endTime);
        bookings.Add(bookingRecord);
            
        foreach (var room in Rooms.ConferenceRooms)
        {
            if (room.Id == roomId)
            {
                room.IsAvailable = false;
                break;
            }
        }
            
        availableRooms.Remove(roomToBook);
        unavailableRooms.Add(roomToBook);
          
        Console.WriteLine($"\n Booking successful!");
        Console.WriteLine($"Room: {roomToBook.Name} (ID: {roomToBook.Id})");
        Console.WriteLine($"Booked by User ID: {userId}");
        Console.WriteLine($"From: {startTime:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"To: {endTime:yyyy-MM-dd HH:mm}");
        Console.WriteLine();
           
        Console.Write("Press any key to continue...");
        Console.ReadKey();
    }
        
    public static void ShowAllBookings(List<Booking> bookings)
    {
        Console.WriteLine("\n=== All Bookings ===");
        
        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings found.");
            return;
        }
        
        foreach (var booking in bookings)
        {
            string roomName = "Unknown Room";
            RoomType roomType = RoomType.Standard;
            foreach (var room in Rooms.ConferenceRooms)
            {
                if (room.Id == booking.Room.Id)
                {
                    roomName = room.Name;
                    roomType = room.Type;
                    break;
                }
            }

            Console.WriteLine($"Room: {booking.Room.Name} Room Type: {booking.Room.Type} (ID: {booking.Room.Id})");
            Console.WriteLine($"User: {booking.UserId}, From: {booking.StartTime:yyyy-MM-dd HH:mm} To: {booking.EndTime:yyyy-MM-dd HH:mm} Booking Status: {booking.Status}");
            Console.WriteLine("---");
        }
    }
    
    public static void CancelBooking(
        List<ConferenceRoom> availableRooms,
        List<ConferenceRoom> unavailableRooms,
        List<Booking> bookings)
    {
        Console.WriteLine("\n=== Cancel Booking ===");
        
        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings to cancel.");
            return;
        }
        
        Console.WriteLine("\nCurrent Bookings:");
        Console.WriteLine("Index\tRoom ID\tUser ID\tStart Time\t\tEnd Time");
        Console.WriteLine("--------------------------------------------------------");
        
        for (int i = 0; i < bookings.Count; i++)
        {
            var booking = bookings[i];
            Console.WriteLine($"{i}\t{booking.Room.Id}\t{booking.UserId}\t{booking.Status}\t{booking.StartTime:yyyy-MM-dd HH:mm}\t{booking.EndTime:yyyy-MM-dd HH:mm}");
        }
        
        Console.Write("\nEnter the index of the booking to cancel (or -1 to cancel): ");
        string indexInput = Console.ReadLine();
        
        if (!int.TryParse(indexInput, out int index) || index < 0 || index >= bookings.Count)
        {
            Console.WriteLine("Invalid index. Cancellation cancelled.");
            return;
        }
        
        var bookingToCancel = bookings[index];
        
        bookingToCancel.Status = BookingStatus.Cancelled;
        
        var roomToUpdate = unavailableRooms.FirstOrDefault(r => r.Id == bookingToCancel.Room.Id);
        if (roomToUpdate != null)
        {
            roomToUpdate.IsAvailable = true;
            unavailableRooms.Remove(roomToUpdate);
            availableRooms.Add(roomToUpdate);
            
            foreach (var room in Rooms.ConferenceRooms)
            {
                if (room.Id == bookingToCancel.Room.Id)
                {
                    room.IsAvailable = true;
                    break;
                }
            }
        }
        
        Console.WriteLine($"\n Booking cancelled successfully for Room ID: {bookingToCancel.Room.Id}");
        Console.WriteLine($"Status updated to: {bookingToCancel.Status}");
    }
}
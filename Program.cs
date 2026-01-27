
List<ConferenceRoom> availableRooms = Rooms.ConferenceRooms.Where(r => r.IsAvailable).ToList();
List<ConferenceRoom> unavailableRooms = Rooms.ConferenceRooms.Where(r => !r.IsAvailable).ToList();
List<Booking> bookings = new List<Booking>();

bool loggedIn = true;

while (loggedIn)
{
    //Console.Clear();
    Console.WriteLine("Welcome to the Conference Booking System");
    Console.WriteLine("---------------------------------------");
    Console.WriteLine("1. List Available Conference Rooms");
    Console.WriteLine("2. List Unavailable Conference Rooms");
    Console.WriteLine("3. Book a Conference Room");
    Console.WriteLine("4. Show All Bookings");
    Console.WriteLine("5. Cancel Booking");
    Console.WriteLine("6. Logout");
    Console.WriteLine("---------------------------------------");
    Console.Write("Please select an option (1-6): ");

    string input = Console.ReadLine();

    switch (input)
    {
        case "1":
            Console.WriteLine();
            Console.WriteLine("Available Conference Rooms:");
            Console.WriteLine("---------------------------");
            foreach (var room in availableRooms)
            {
                Console.WriteLine($"ID: {room.Id}, Name: {room.Name}, Capacity: {room.Capacity}");
            }
            Console.WriteLine();
            break;
        case "2":
            Console.WriteLine();
            Console.WriteLine("Unavailable Conference Rooms:");
            Console.WriteLine("-----------------------------");
            foreach (var room in unavailableRooms)
            {
                Console.WriteLine($"ID: {room.Id}, Name: {room.Name}, Capacity: {room.Capacity}");
            }
            Console.WriteLine();
            break;
        case "3":
            CreateBooking.BookConferenceRoom(availableRooms, unavailableRooms, bookings);
            break;
        case "4":
            CreateBooking.ShowAllBookings(bookings);
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
            break;
        case "5":
            CreateBooking.CancelBooking(availableRooms, unavailableRooms, bookings);
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
            break;
        case "6":
            loggedIn = false;
            Console.WriteLine("You have been logged out.");
            Console.WriteLine("Goodbye!");
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }

    
}
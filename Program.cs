
bool loggedIn = true;

while (loggedIn)
{
    //Console.Clear();
    Console.WriteLine("Welcome to the Conference Booking System");
    Console.WriteLine("---------------------------------------");
    Console.WriteLine("1. List Available Conference Rooms");
    Console.WriteLine("2. List Unavailable Conference Rooms");
    Console.WriteLine("3. Book a Conference Room");
    Console.WriteLine("4. Logout");
    Console.WriteLine("---------------------------------------");
    Console.Write("Please select an option (1-4): ");

    string input = Console.ReadLine();

    switch (input)
    {
        case "1":
            Console.WriteLine("Action performed.");
            break;
        case "2":
            Console.WriteLine("Action performed.");
            break;
        case "3":
            Console.WriteLine("Action performed.");
            break;
        case "4":
            loggedIn = false;
            Console.WriteLine("You have been logged out.");
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }

    Console.WriteLine("Goodbye!");
}
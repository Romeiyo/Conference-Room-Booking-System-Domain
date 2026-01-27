# Conference-Room-Booking-System-Domain
## Project Overview
This is a domain model implementation for a Conference Room Booking System using C#. The project focuses on modelling core business concepts and rules that will later power a Conference Room Booking System API.

## Assignment Requirements
This project was developed as part of a domain modelling assignment with the following requirements:

- Design a clean, intentional domain model using C#
- Represent core concepts of a Conference Room Booking System
- Include appropriate C# constructs (class, record, enum) based on intent
- Use constructors to enforce valid object state
- Avoid public fields; use properties intentionally
- Encapsulate behaviour inside domain objects where appropriate
- Prevent invalid states through enums or logic
- Use meaningful names that reflect business language

## Domain Model Components
## Core Domain Classes
ConferenceRoom Class
Represents a conference room with the following properties:

- Id: Unique identifier (positive integer)
- Name: Room name (required, non-empty)
- Capacity: Room capacity (positive integer)
- IsAvailable: Availability status

### Key Features:

- Constructor with validation to enforce valid state
- Domain methods: BookRoom() and ReleaseRoom() for managing availability
- Encapsulated business logic for room management

## Booking Class
### Represents a room booking with the following properties:

- RoomId: Reference to booked room (positive integer)
- UserId: User making the booking (positive integer)
- StartTime: Booking start time
- EndTime: Booking end time (must be after start time)

## Key Features:

- Constructor with comprehensive validation
- Business method: OverlapsWith() for checking booking conflicts
- Enforces that end time must be after start time

## Supporting Classes
### Rooms Static Class
- Provides a static collection of ConferenceRoom objects
- Pre-populated with 16 sample conference rooms
- Serves as an in-memory data repository

### BookingFunction Static Class
- Contains business logic for booking operations
- Methods for booking rooms, showing bookings, and cancelling bookings
- Handles user input and console interactions

## Constructor Validation
Both domain classes use constructors to enforce valid state:

- Prevents creation of objects with invalid data
- Throws meaningful exceptions for validation failures
- Ensures domain invariants are maintained

## Property Design
- All properties use getters and setters intentionally
- No public fields - encapsulation is maintained
- Properties reflect business terminology


## Separation of Concerns
- Domain logic is encapsulated in domain objects
- UI/Console logic is separated in BookingFunction class
- Data persistence is abstracted in Rooms class
- Business Rules Implemented

### Room Validation Rules:

- Room ID must be positive
- Room name is required
- Capacity must be positive
- Booking Validation Rules:
- Room and User IDs must be positive
- End time must be after start time
- No overlapping bookings for the same room
- Room Management Rules:
- Cannot book an unavailable room
- Cancelled bookings free up room availability

## Console Application Features
The supporting console application provides:

- List Available Rooms: View all currently available conference rooms
- List Unavailable Rooms: View all currently booked rooms
- Book a Conference Room: Make a new booking with validation
- Show All Bookings: View all existing bookings
- Cancel Booking: Cancel an existing booking
- Technical Implementation Details
Validation
- Input validation for all user inputs
- Business rule validation in domain constructors
- Overlap checking for booking conflicts

## How to Run
- Ensure you have .NET SDK installed
- Clone the repository
- Navigate to the project directory
- Run: dotnet run

## Author
- Email: romeopomeo1@gmail.com
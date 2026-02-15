# 🏢 Conference-Room-Booking-System-Domain

This is a domain model implementation for a Conference Room Booking System using C#. The project focuses on modelling core business concepts and rules that will later power a Conference Room Booking System API. The Project has been further expanded upon to include Booking functionalities using mock data.

---

## 📌 Purpose of this Repository

This repository is used for:
- Gradually improving project code over time
- Created the Conference booking domain
- Using lists and LINQ to manage booking data

---

## Table of Contents
- [Repository Contents](#-repository-contents)
- [Installation](#️-installation)
- [Usage](#-usage)
- [Contributing](#-contributing)
- [System Context](#system-context)
- [License](#license)
- [Author](#author)

---

## 🗂 Repository Contents

- [README file](README.md) - Project Overview.
- [Mock Data](data/Rooms.cs) - Mock data to use for booking rooms.
- [Enums](enums/Enums.cs) - Project enums.
- [Methods](methods/BookingFunction.cs) - Project methods.
- [Models](models/) - Project Classes.

---

## ⚙️ Installation

- Ensure you have .NET SDK [installed(install here)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Copy the HTTPS link
- Clone the repository using `git clone [Project HTTPS link]`
- Navigate to the project directory
- Run: dotnet run

---

## 🚀 Usage

This repository is currently used for:
1. Creating project domain
2. Working with collections

---

## 🤝 Contributing

Changes to this repository are made using **Pull Requests**.

Contributors should:
- Fork the repository
- Create a feature or documentation branch
- Submit changes via a Pull Request
- Clearly describe the intent of the change

---

## System Context

### Key Features:

- Constructor with validation to enforce valid state
- Domain methods: BookRoom() and ReleaseRoom() for managing availability
- Encapsulated business logic for room management

### Booking Class
### Represents a room booking with the following properties:

- Room: Reference to booked room (positive integer)
- UserId: User making the booking (positive integer)
- StartTime: Booking start time
- EndTime: Booking end time (must be after start time)

### Supporting Classes
### Rooms Static Class
- Provides a static collection of ConferenceRoom objects
- Pre-populated with 16 sample conference rooms
- Serves as an in-memory data repository

### BookingFunction Static Class
- Contains business logic for booking operations
- Methods for booking rooms, showing bookings, and cancelling bookings
- Handles user input and console interactions

### Constructor Validation
Both domain classes use constructors to enforce valid state:

- Prevents creation of objects with invalid data
- Throws meaningful exceptions for validation failures
- Ensures domain invariants are maintained

### Separation of Concerns
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

### Console Application Features
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

### Serialization & Deserialization
- Added a Serialization method to download bookings that were made
- Added a Deserialization method to load bookings from an external json file

---

# API Querying, Filtering & Pagination

### Response Format

Every paginated endpoint returns the same consistent structure:

- **totalCount** - Total number of records matching your query
- **page** - Current page number
- **pageSize** - Number of items per page
- **data** - Array of items for the current page

**Important:** All pagination happens at the database level. Only the requested page of data is ever transferred from the database to the application.

## Performance Considerations

### Database-Level Operations

All filtering, sorting, and pagination happens **inside the database**, not in your application memory. This means:

- The database does the heavy lifting
- Your application only receives the data it needs
- No wasted memory loading thousands of records you'll never use

### Summary

| Feature | Status | Benefit |
|---------|--------|---------|
| Pagination at database level | ✅ Yes | Only requested page is loaded |
| Filtering at database level | ✅ Yes | Uses WHERE clauses, not in-memory filtering |
| Sorting at database level | ✅ Yes | Uses ORDER BY, not in-memory sorting |
| Read-only optimization | ✅ Yes | AsNoTracking() reduces overhead |
| Column projection | ✅ Yes | Only retrieves needed fields |
| In-memory operations | ❌ None | Everything happens in the database |

The result is an API that is **fast, efficient, and production-ready** from day one.

---

## License
This project is [LICENSED](LICENSE) under the MIT License.

## Author
Tumisang (Romio) Lesimola: romeopomeo1@gmail.com  
Created as part of professional software development training.
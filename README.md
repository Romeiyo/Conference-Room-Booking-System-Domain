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

## How Pagination Works

All list endpoints support pagination through two optional query parameters:

- **page** - The page number to retrieve (default: 1, starts at 1)
- **pageSize** - Number of items per page (default: 10)
- **sortBy** - What order to sort by (default: null)


### The Pagination Process

1. **Count** - First, the database counts how many total records match your query
2. **Skip** - Calculates how many records to skip based on the current page
3. **Take** - Retrieves only the requested page size from the database
4. **Response** - Returns metadata together with the requested page of data

### Response Format

Every paginated endpoint returns the same consistent structure:

- **totalCount** - Total number of records matching your query
- **page** - Current page number
- **pageSize** - Number of items per page
- **data** - Array of items for the current page

**Important:** All pagination happens at the database level. Only the requested page of data is ever transferred from your database to your application.


## Supported Filters & Sorting

### Booking Sorting Endpoints

| Endpoint | Description | Sort Order |
|----------|-------------|------------|
| `/api/sorting/bookings/roomName` | All bookings by room name | A to Z |
| `/api/sorting/bookings/roomName/desc` | All bookings by room name | Z to A |
| `/api/sorting/bookings/startTime` | All bookings by start time | Oldest first |
| `/api/sorting/bookings/startTime/desc` | All bookings by start time | Newest first |
| `/api/sorting/bookings/createdAt` | All bookings by creation date | Oldest first |
| `/api/sorting/bookings/createdAt/desc` | All bookings by creation date | Newest first |
| `/api/sorting/bookings/capacity` | All bookings by room capacity | Smallest first |
| `/api/sorting/bookings/capacity/desc` | All bookings by room capacity | Largest first |
| `/api/sorting/bookings/status/Cancelled` | All cancelled bookings | Most recently cancelled first |

### Booking Filtering Endpoints

| Endpoint | Description | Default Sort |
|----------|-------------|--------------|
| `/api/sorting/bookings/location/{location}` | Filter by room location | By start time |
| `/api/sorting/bookings/room/{roomId}` | Filter by specific room | By start time |
| `/api/sorting/bookings/date/{year}/{month}/{day}` | Filter by specific date | By start time |
| `/api/sorting/bookings/date-range/{from}/{to}` | Filter by date range | By start time |
| `/api/sorting/bookings/status/{status}` | Filter by booking status | By start time |
| `/api/sorting/bookings/roomType/{roomType}` | Filter by room type | By start time |
| `/api/sorting/bookings/user/{userId}` | Filter by user (Admin only) | Newest first |
| `/api/sorting/my-bookings` | Current user's bookings | Newest first |

**Valid Status Values:** `Booked`, `Confirmed`, `Cancelled`, `InProgress`

**Valid Room Types:** `Standard`, `Boardroom`, `Training`

### Room Endpoints

| Endpoint | Description | Sort Order |
|----------|-------------|------------|
| `/api/sorting/rooms/name` | All active rooms by name | A to Z |
| `/api/sorting/rooms/capacity` | All active rooms by capacity | Smallest first |
| `/api/sorting/rooms/capacity/desc` | All active rooms by capacity | Largest first |
| `/api/sorting/rooms/location/{location}` | Filter rooms by location | By name |
| `/api/sorting/rooms/type/{type}` | Filter rooms by type | By name |


## Performance Considerations

### Database-Level Operations

All filtering, sorting, and pagination happens **inside the database**, not in your application memory. This means:

- The database does the heavy lifting
- Your application only receives the data it needs
- No wasted memory loading thousands of records you'll never use

### Key Performance Optimizations

| Optimization | What It Does |
|-------------|--------------|
| **Database-level pagination** | Only retrieves the requested page (e.g., 10 records) instead of all records (e.g., 10,000) |
| **No early data loading** | Keeps queries as queries until the last moment - all operations happen in SQL |
| **Read-only queries** | Disables change tracking for list views, reducing memory overhead |
| **Projection** | Only selects the specific columns needed for list views, not entire entities |
| **Separate count query** | Gets total record count with a lightweight COUNT(*) query without loading data |
| **Index-friendly sorting** | Sorts on database columns that can be indexed for faster performance |

### What This Means For You

- **With 10 records** - Fast response
- **With 10,000 records** - Still fast response (same 10 records loaded)
- **With 1,000,000 records** - Maintains performance (still only 10 records loaded)

The API will remain fast and responsive regardless of how large your database grows.

### Memory Usage Comparison

| Approach | Records in Database | Memory Used | Response Time |
|----------|---------------------|-------------|---------------|
| **Without pagination** | 10,000 | Loads all 10,000 records | Slow, crashes at scale |
| **With client-side pagination** | 10,000 | Still loads all 10,000 records | Slow, memory intensive |
| **Our implementation** | 10,000 | **Loads only 10 records** | Fast, scales infinitely |

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
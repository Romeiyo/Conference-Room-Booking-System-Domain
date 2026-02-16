function BookingCard({ booking }) {
    return (
        <div style={{ border: "1px solid #ccc", padding: "10px", margin: "5px" }}>
            <h3>{booking.roomName}</h3>
            <p><strong>Date:</strong> {booking.date}</p>
            <p><strong>Time:</strong> {booking.startTime} - {booking.endTime}</p>
            <p><strong>Status:</strong> {booking.status}</p>
            <p><strong>Location:</strong> {booking.location}</p>
            <p><strong>Booked By:</strong> {booking.bookedBy}</p>
        </div>
    );
}

export default BookingCard;
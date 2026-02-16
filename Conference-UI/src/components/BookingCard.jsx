import '../App.css';

function BookingCard({ booking }) {
    return (
        <div className='booking-card'>
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
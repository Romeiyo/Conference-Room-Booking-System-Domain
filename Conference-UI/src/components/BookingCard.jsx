import '../App.css';

function BookingCard({ booking }) {
    const statusColor = {
        'Confirmed': '#e8f5e8',
        'Cancelled': '#ffebee'
    }
    return (
        <div className='booking-card'>
            <h3>{booking.roomName}</h3>
            <p><strong>Date:</strong> {booking.date}</p>
            <p><strong>Time:</strong> {booking.startTime} - {booking.endTime}</p>
            <p style={{ 
                backgroundColor: statusColor[booking.status] || '#e8f5e8',
                color: booking.status === 'Confirmed' ? '#2e7d32' : 
                       booking.status === 'Pending' ? '#858585' : '#c62828'
            }}><strong>Status:</strong> {booking.status}</p>
            <p><strong>Location:</strong> {booking.location}</p>
            <p><strong>Booked By:</strong> {booking.bookedBy}</p>
        </div>
    );
}

export default BookingCard;
import '../App.css';

function BookingCard({ booking }) {
    const statusColor = {
        'Confirmed': '#e8f5e8',
        'Cancelled': '#ffebee',
        'Booked': '#e8f5e8'
    };

    // Handle both possible data formats (API response or mock data)
    const roomName = booking.room?.name || booking.roomName || 'Unknown Room';
    const location = booking.room?.location || booking.location || 'Unknown Location';
    const date = booking.bookingDate || booking.date || 'No date';
    const startTime = booking.startTime || '';
    const endTime = booking.endTime || '';
    const status = booking.status || 'Pending';
    const bookedBy = booking.bookedBy || 'Unknown';

    // Format time to remove seconds if needed (e.g., "01:28:00" -> "01:28")
    const formatTime = (time) => {
        if (!time) return '';
        return time.length > 5 ? time.substring(0, 5) : time;
    };

    return (
        <div className='booking-card'>
            <h3>{roomName}</h3>
            <p><strong>Date:</strong> {date}</p>
            <p><strong>Time:</strong> {startTime} - {endTime}</p>
            <p style={{ 
                backgroundColor: statusColor[status] || '#e8f5e8',
                color: booking.status === 'Confirmed' ? '#2e7d32' : 
                       booking.status === 'Booked' ? '#2e7d32' :
                       booking.status === 'Pending' ? '#858585' : '#c62828'
            }}><strong>Status:</strong> {status}</p>
            <p><strong>Location:</strong> {location}</p>
            <p><strong>Booked By:</strong> {bookedBy}</p>
        </div>
    );
}

export default BookingCard;
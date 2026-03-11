import '@/app/globals.css';

interface Booking {
    id: number;
    roomName?: string;
    room?: { name: string; location: string };
    location?: string;
    bookingDate?: string;
    date?: string;
    startTime?: string;
    endTime?: string;
    status: string;
    bookedBy?: string;
}

interface BookingCardProps {
    booking: Booking;
}

export default function BookingCard({ booking }: BookingCardProps) {
    const statusColor = {
        'Confirmed': '#e8f5e8',
        'Cancelled': '#ffebee',
        'Booked': '#e8f5e8'
    };

    const roomName = booking.room?.name || booking.roomName || 'Unknown Room';
    const location = booking.room?.location || booking.location || 'Unknown Location';
    const date = booking.bookingDate || booking.date || 'No date';
    const startTime = booking.startTime || '';
    const endTime = booking.endTime || '';
    const status = booking.status || 'Pending';
    const bookedBy = booking.bookedBy || 'Unknown';

    const formatTime = (time: string) => {
        if (!time) return '';
        return time.length > 5 ? time.substring(0, 5) : time;
    };

    return (
        <div className='booking-card'>
            <h3>{roomName}</h3>
            <p><strong>Date:</strong> {date}</p>
            <p><strong>Time:</strong> {formatTime(startTime)} - {formatTime(endTime)}</p>
            <p style={{ 
                backgroundColor: statusColor[status as keyof typeof statusColor] || '#e8f5e8',
                color: status === 'Confirmed' ? '#2e7d32' : 
                       status === 'Booked' ? '#2e7d32' :
                       status === 'Pending' ? '#858585' : '#c62828'
            }}><strong>Status:</strong> {status}</p>
            <p><strong>Location:</strong> {location}</p>
            <p><strong>Booked By:</strong> {bookedBy}</p>
        </div>
    );
}
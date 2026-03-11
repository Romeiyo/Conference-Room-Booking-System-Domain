import BookingCard from './BookingCard';
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

interface BookingListProps {
    bookings: Booking[];
    selectedBookingId?: number | null;
    onSelectBooking: (id: number) => void;
}

export default function BookingList({ bookings, selectedBookingId, onSelectBooking }: BookingListProps) {
    return (
        <div className='booking-list'>
            <h2>Current Bookings</h2>
            {bookings.map(booking => (
                <div 
                    key={booking.id}
                    className={`booking-wrapper ${selectedBookingId === booking.id ? 'selected' : ''}`}
                    onClick={() => onSelectBooking(booking.id)}>
                        <BookingCard booking={booking}/>
                </div>
            ))}
        </div>
    );
}
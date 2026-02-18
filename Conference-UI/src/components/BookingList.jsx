import BookingCard from './BookingCard';
import '../App.css';

function BookingList({ bookings, selectedBookingId, onSelectBooking }) {
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
                // <BookingCard key={booking.id} booking={booking} />
            ))}
        </div>
    );
}

export default BookingList;
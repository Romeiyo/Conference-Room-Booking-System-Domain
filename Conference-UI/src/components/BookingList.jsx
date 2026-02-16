import BookingCard from './BookingCard';
import '../App.css';

function BookingList({ bookings }) {
    return (
        <div className='booking-list'>
            <h2>Current Bookings</h2>
            {bookings.map(booking => (
                <BookingCard key={booking.id} booking={booking} />
            ))}
        </div>
    );
}

export default BookingList;
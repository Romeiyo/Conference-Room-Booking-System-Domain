import BookingCard from './BookingCard';

function BookingList({ bookings }) {
    return (
        <div>
            <h2>Current Bookings</h2>
            {bookings.map(booking => (
                <BookingCard key={booking.id} booking={booking} />
            ))}
        </div>
    );
}

export default BookingList;
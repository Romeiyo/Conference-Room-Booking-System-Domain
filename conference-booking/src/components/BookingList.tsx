import BookingCard from './BookingCard';

export default function BookingList({ bookings, selectedId, onSelect }: any) {
  return (
    <div>
      <h2 className="text-xl font-semibold mb-4">Current Bookings</h2>
      {bookings.map((booking: any) => (
        <div
          key={booking.id}
          onClick={() => onSelect(booking.id)}
          className={`cursor-pointer transition ${
            selectedId === booking.id ? 'ring-2 ring-blue-500' : ''
          }`}
        >
          <BookingCard booking={booking} />
        </div>
      ))}
    </div>
  );
}
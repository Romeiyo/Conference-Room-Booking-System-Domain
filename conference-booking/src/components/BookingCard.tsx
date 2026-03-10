export default function BookingCard({ booking }: any) {
  const statusColor: Record<string, string> = {
    'Confirmed': '#e8f5e8',
    'Cancelled': '#ffebee'
  };

  return (
    <div className="border rounded-lg p-4 mb-2 hover:shadow-md transition">
      <h3 className="font-bold text-lg">{booking.roomName}</h3>
      <p><strong>Date:</strong> {booking.date}</p>
      <p><strong>Time:</strong> {booking.startTime} - {booking.endTime}</p>
      <p style={{ 
        backgroundColor: statusColor[booking.status as string] || '#e8f5e8',
        color: booking.status === 'Confirmed' ? '#2e7d32' : 
               booking.status === 'Pending' ? '#858585' : '#c62828'
      }}>
        <strong>Status:</strong> {booking.status}
      </p>
      <p><strong>Location:</strong> {booking.location}</p>
      <p><strong>Booked By:</strong> {booking.bookedBy}</p>
    </div>
  );
}
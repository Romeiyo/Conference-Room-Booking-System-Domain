import { useState, useEffect } from 'react';
import Navbar from './components/Navbar';
import Footer from './components/footer';
import BookingList from './components/BookingList';
import Button from './components/Button';
import initialBookings from './mockData';
import BookingForm from './components/BookingForm';
import './App.css'


function App() {

  const [bookings, setBookings] = useState(() => {
    const savedBookings = localStorage.getItem('bookings');
    
    if (savedBookings)
    {
      return JSON.parse(savedBookings);
    }

    return initialBookings;
  });

  const [selectedBookingId, setSelectedBookingId] = useState(null);

  useEffect(() => {
    localStorage.setItem('bookings', JSON.stringify(bookings));
  }, [bookings]);

  const addBooking = (newBooking) => {

    const bookingWithStatus = {
      ...newBooking,
      status: 'Confirmed'
    };

    setBookings([...bookings, bookingWithStatus]);
  
  }

  //function to select cards
  const handleSelectBooking = (bookingId) => {
    setSelectedBookingId(bookingId);
  }

  const cancelSelectedBooking = () => {
    if (!selectedBookingId)
    {
      alert("Please select a booking to cancel");
      return;
    }

    if (window.confirm('Are you sure you want to cancel this selected booking?'))
    {
      setBookings(bookings.filter(booking => booking.id !== selectedBookingId));
      setSelectedBookingId(null);
    }
  }
  
  const totalBookings = bookings.length;

  return (
    <div className='app-container'>
      <Navbar />
        <main className='main-content'>
          <p>Total Bookings: {totalBookings}</p>
          {selectedBookingId && (
            <p className='selected-info'>Selected Booking ID: {selectedBookingId}</p>
          )}
          <BookingForm onAddBooking={addBooking}/>
          <BookingList 
            bookings={bookings}
            selectedBookingId={selectedBookingId}
            onSelectBooking={handleSelectBooking}
            />
            <div style ={{ textAlign: "center", marginTop: "2rem"}}>
              <Button 
                label={selectedBookingId ? "Cancel selected Booking" : "Cancel Booking"} 
                onClick={cancelSelectedBooking} />
            </div>
        </main>
      <Footer />
    </div>
  );
}

export default App

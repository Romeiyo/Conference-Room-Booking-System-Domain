import { useState, useEffect } from 'react';
import Navbar from './components/Navbar';
import Footer from './components/footer';
import BookingList from './components/BookingList';
import Button from './components/Button';
import BookingForm from './components/BookingForm';
import { fetchAllBookings } from './Services/bookingService';
import Filter from './components/Filter';
import './App.css'

function App() {

  const [bookings, setBookings] = useState([]);
  const [filteredBookings, setFilteredBookings] = useState([]);
  const [selectedBookingId, setSelectedBookingId] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [retryCount, setRetryCount] = useState(0);
  const [categoryFilter, setCategoryFilter] = useState('All');

  // Fetch bookings from my "API" when component mounts
  useEffect(() => {
    const loadBookings = async () =>{
      try {
        setLoading(true);
        setError(null);
        const data = await fetchAllBookings();
        setBookings(data);
      }catch (err) {
        setError("Za buukings culudo notu be loooded!");

        const savedBookings = localStorage.getItem('bookings');
        if (savedBookings) {
          setBookings(JSON.parse(savedBookings));
        }
      }finally{
        setLoading(false);
      }
    };
    loadBookings();
  }, [retryCount]);

  // Filter bookings when category changes OR when bookings change
  useEffect(() => {

    if (categoryFilter === 'All') {
      setFilteredBookings(bookings);
    } else {
      // Filter by location
      const filtered = bookings.filter(
        booking => booking.location === categoryFilter
      );
      setFilteredBookings(filtered);
    }
    
    // using SetFilteredBookings instead of setBookings prevents infinate loops
  }, [categoryFilter, bookings]); 


  // saves bookings to local storage(persistance), not normal bookings
  useEffect(() => {
    localStorage.setItem('bookings', JSON.stringify(bookings));
  }, [bookings]);

  // Get unique locations for filter dropdown
  const locations = ['All', ...new Set(bookings.map(b => b.location))];

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

  const handleRetry = () => {
    setRetryCount(prev => prev + 1);
  }

  // Handler for category filter change
  const handleCategoryChange = (event) => {
    setCategoryFilter(event.target.value);
  }
  
  const totalBookings = bookings.length;

  // this loads while app is fetching api data
  if (loading && bookings.length === 0) {
    return(
      <div className='app-container'>
        <Navbar/>
        <main className='main-content'>
          <div style = {{textAlign: 'center', padding: '3rem'}}>
            <h2>Loading Bookings...</h2>
            <p>This might take a moment....please wait</p>
          </div>
        </main>
        <Footer/>
      </div>
    );
  }

  // Error state with retry option
  // this loads when api fails
  if (error && bookings.length === 0) {
    return (
      <div className='app-container'>
        <Navbar />
        <main className='main-content'>
          <div style={{ 
            textAlign: 'center', 
            padding: '3rem',
            background: '#ffebee',
            borderRadius: '8px',
            color: '#c62828'
          }}>
            <h2>Oops! Something went wrong</h2>
            <p>{error}</p>
            <p>The API failed to load bookings.</p>
            {retryCount < 30 && (
              <Button label="Retry Loading" onClick={handleRetry} />
            )}
          </div>
        </main>
        <Footer />
      </div>
    );
  }

  // this loads when api returns bookings
  return (
    <div className='app-container'>
      <Navbar />
      <main className='main-content'>
        {/* Total Bookings counter */}
        <div className="total-bookings">
          Total Bookings: {totalBookings}
        </div>

        {/* Split layout */}
        <div className="dashboard-layout">
          {/* Left Column - Fixed Form */}
          <div className="form-column">
            <BookingForm onAddBooking={addBooking}/>
            
            <div style={{ marginTop: "1rem" }}>
              <Button 
                label={selectedBookingId ? "Cancel selected Booking" : "Cancel Booking"} 
                onClick={cancelSelectedBooking} 
              />
            </div>
            
            {/* Refresh button */}
            <div style={{ marginTop: "1rem" }}>
              <Button 
                label="Refresh Bookings" 
                onClick={() => setRetryCount(prev => prev + 1)}/>
            </div>
          </div>

          {/* Right Column - Scrollable Bookings */}
          <div className="bookings-column">
            {/* Filter (moves with scroll in right column) */}
            <Filter 
              categoryFilter={categoryFilter}
              onCategoryChange={handleCategoryChange}
              locations={locations}
            />

            {loading && (
              <p className="loading-text">Refreshing data...</p>
            )}
            
            {selectedBookingId && (
              <p className='selected-info'>Selected Booking ID: {selectedBookingId}</p>
            )}
            
            <BookingList 
              bookings={filteredBookings}
              selectedBookingId={selectedBookingId}
              onSelectBooking={handleSelectBooking}
            />
          </div>
        </div>
      </main>
      <Footer />
    </div>
  );
}

export default App

import Navbar from './components/Navbar';
import Footer from './components/footer';
import BookingList from './components/BookingList';
import Button from './components/Button';
import bookings from './mockData';
//import './App.css'

function App() { 

  return (
    <div>
      <Navbar />

      <BookingList bookings={bookings}/>
      
      <Button label="Cancel Booking"/>
      
      <Footer />
    </div>
  );
}

export default App

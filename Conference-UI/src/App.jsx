import Navbar from './components/Navbar';
import Footer from './components/footer';
import BookingList from './components/BookingList';
import Button from './components/Button';
import bookings from './mockData';
import './App.css'

function App() { 

  return (
    <div className='app-container'>
      <Navbar />
        <main className='main-content'>
          <BookingList bookings={bookings}/>
            <div style ={{ textAlign: "center", marginTop: "2rem"}}>
              <Button 
                label="Cancel Booking" 
                onClick={() => alert("Select Booking to Cancel")} />
            </div>
        </main>
      <Footer />
    </div>
  );
}

export default App

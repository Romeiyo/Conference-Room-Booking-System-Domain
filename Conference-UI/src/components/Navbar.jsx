import { useEffect } from 'react';
import '../App.css';

function Navbar() {

    // Heartbeat effect
    useEffect(() => {
      // Set up the interval
      const heartbeat = setInterval(() => {
        console.log(`checking for updates... <3`);
      }, 3000); // Every 3 seconds
    
      // Cleanup function to clear interval when component unmounts
      return () => {
        clearInterval(heartbeat);
        
        console.log(`Heartbeat stopped - Navbar unmounted </3`);
      };
    }, []); // Empty dependency array = runs once on mount
    
    return (
        <nav className="navbar">
            <h1>Conference Room Booking System</h1>
            <ul>
                <li><a href="/">Home</a></li>
                <li><a href="/bookings">Bookings</a></li>
                <li><a href="/rooms">Rooms</a></li>
                <li><a href="/contact">Contact</a></li>
            </ul>
        </nav>     
    );
}

export default Navbar;
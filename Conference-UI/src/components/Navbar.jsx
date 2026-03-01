import { useEffect, useRef } from 'react';
import '../App.css';
import { useAuth } from '../hooks/useAuth';

function Navbar() {
    const { isAuthenticated, user, logout } = useAuth(); // Now this will work
    const heartbeatInterval = useRef(null);
    // Log when Navbar mounts/unmounts
    useEffect(() => {
        console.log('✅ Navbar MOUNTED');
        
        return () => {
            console.log('❌ Navbar UNMOUNTED');
        };
    }, []);

    // Separate heartbeat effect
    useEffect(() => {
        console.log('💓 Heartbeat started');
        const heartbeat = setInterval(() => {
            console.log(`checking for updates... <3`);
        }, 3000);
        
        return () => {
            clearInterval(heartbeat);
            console.log('💔 Heartbeat stopped');
        };
    }, []);

    const handleLogout = () => {
        logout();
    };

    return (
        <nav className="navbar">
            <h1>Conference Room Booking System</h1>
            <ul>
                <li><a href="/">Home</a></li>
                <li><a href="/bookings">Bookings</a></li>
                <li><a href="/rooms">Rooms</a></li>
                <li><a href="/contact">Contact</a></li>
                {isAuthenticated && (
                    <li>
                        <span style={{ color: 'white', marginRight: '1rem' }}>
                            Welcome, {user?.username}
                        </span>
                        <button 
                            onClick={handleLogout}
                            style={{
                                background: 'transparent',
                                border: '1px solid white',
                                color: 'white',
                                padding: '0.3rem 1rem',
                                borderRadius: '4px',
                                cursor: 'pointer'
                            }}
                        >
                            Logout
                        </button>
                    </li>
                )}
            </ul>
        </nav>     
    );
}

export default Navbar;
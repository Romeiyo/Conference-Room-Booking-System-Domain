"use client";

import Link from 'next/link';
import { useAuth } from '@/context/AuthContext';
import '@/app/globals.css';

export default function Navbar() {
    const { isAuthenticated, user, logout } = useAuth();

    return (
        <nav className="navbar">
            <h1>Conference Room Booking System</h1>
            <ul>
                <li><Link href="/">Home</Link></li>
                <li><Link href="/dashboard">Dashboard</Link></li>
                <li><Link href="/bookings">Bookings</Link></li>
                {isAuthenticated ? (
                    <li>
                        <span style={{ color: 'white', marginRight: '1rem' }}>
                            Welcome, {user?.username}
                        </span>
                        <button 
                            onClick={logout}
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
                ) : (
                    <li>
                        <Link href="/login">Login</Link>
                    </li>
                )}
            </ul>
        </nav>
    );
}
"use client";

import { useEffect, useRef } from 'react';
import Link from 'next/link';
import { useAuth } from '@/hooks/useAuth';
import '@/app/globals.css';

export default function Navbar() {
    const { isAuthenticated, user, logout } = useAuth();
    const heartbeatInterval = useRef(null);

    useEffect(() => {
        console.log('✅ Navbar MOUNTED');
        
        return () => {
            console.log('❌ Navbar UNMOUNTED');
        };
    }, []);

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
                <li><Link href="/">Home</Link></li>
                <li><Link href="/dashboard">Dashboard</Link></li>
                <li><Link href="/bookings">Bookings</Link></li>
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
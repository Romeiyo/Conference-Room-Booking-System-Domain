"use client";

import { useEffect } from 'react';
import Link from 'next/link';

export default function BookingsError({
    error,
    reset,
}: {
    error: Error & { digest?: string };
    reset: () => void;
}) {
    useEffect(() => {
        // Log the error to an error reporting service
        console.error('Bookings page error:', error);
    }, [error]);

    return (
        <div className="container">
            <div className="error-boundary" style={{
                textAlign: 'center',
                padding: '4rem 2rem',
                background: 'white',
                borderRadius: '12px',
                boxShadow: '0 2px 10px rgba(0,0,0,0.1)',
                maxWidth: '600px',
                margin: '2rem auto'
            }}>
                <div style={{ fontSize: '4rem', marginBottom: '1rem' }}>⚠️</div>
                <h1 style={{ color: '#721c24', marginBottom: '1rem' }}>
                    Something went wrong!
                </h1>
                <p style={{ color: '#666', marginBottom: '2rem' }}>
                    {error.message || 'Failed to load bookings. Please try again.'}
                </p>
                
                <div style={{ display: 'flex', gap: '1rem', justifyContent: 'center' }}>
                    <button
                        onClick={reset}
                        className="btn btn-primary"
                        style={{ background: '#667eea' }}
                    >
                        Try Again
                    </button>
                    <Link
                        href="/dashboard"
                        className="btn btn-secondary"
                        style={{ background: '#6c757d', color: 'white', textDecoration: 'none' }}
                    >
                        Go to Dashboard
                    </Link>
                </div>
                
                <p style={{ marginTop: '2rem', fontSize: '0.9rem', color: '#999' }}>
                    If the problem persists, please contact support.
                </p>
            </div>
        </div>
    );
}
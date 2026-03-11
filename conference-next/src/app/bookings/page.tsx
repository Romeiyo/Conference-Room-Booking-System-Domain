"use client";

import ProtectedRoute from '@/components/ProtectedRoute';
import { useBookings } from '@/hooks/useBookings';
import { useAuth } from '@/context/AuthContext';
import BookingList from '@/components/BookingList';
import Filter from '@/components/Filter';
import Link from 'next/link';
import '@/app/globals.css';

function BookingsContent() {
    const {
        filteredBookings,
        isLoading: bookingsLoading,
        error,
        selectedBookingId,
        categoryFilter,
        locations,
        selectBooking,
        cancelSelectedBooking,
        changeFilter,
        retryFetch
    } = useBookings();

    const { user } = useAuth();

    const handleCancelSelected = async () => {
        const success = await cancelSelectedBooking();
        if (success) {
            alert('Booking cancelled successfully');
        }
    };

    return (
        <div className="container">
            <div className="header-section">
                <h1>Conference Room Bookings</h1>
                {user && <p>Welcome, {user.username}!</p>}
                <div className="header-actions">
                    <Link href="/dashboard" className="btn btn-primary">
                        + New Booking
                    </Link>
                    {selectedBookingId && (
                        <button 
                            className="btn btn-danger" 
                            onClick={handleCancelSelected}
                        >
                            Cancel Selected Booking
                        </button>
                    )}
                </div>
            </div>

            {error && (
                <div className="error-container">
                    <p className="error">{error}</p>
                    <button onClick={retryFetch} className="btn btn-secondary">
                        Retry
                    </button>
                </div>
            )}

            <Filter 
                categoryFilter={categoryFilter}
                onCategoryChange={(e) => changeFilter(e.target.value)}
                locations={locations}
            />

            {bookingsLoading ? (
                <div className="loading-container">
                    <div className="spinner"></div>
                    <p>Loading bookings...</p>
                </div>
            ) : (
                <BookingList 
                    bookings={filteredBookings}
                    selectedBookingId={selectedBookingId}
                    onSelectBooking={selectBooking}
                />
            )}
        </div>
    );
}

export default function BookingsPage() {
    return (
        <ProtectedRoute>
            <BookingsContent />
        </ProtectedRoute>
    );
}
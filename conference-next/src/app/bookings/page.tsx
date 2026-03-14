"use client";

import ProtectedRoute from '@/components/ProtectedRoute';
import { useBookings } from '@/hooks/useBookings';
import { useAuth } from '@/context/AuthContext';
import BookingList from '@/components/BookingList';
import Filter from '@/components/Filter';
import SearchBar from '@/components/SearchBar';
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
        retryFetch,
        searchTerm,
        setSearch
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

            <div className="filter-section">
                <div className="filter-group">
                    <Filter 
                        categoryFilter={categoryFilter}
                        onCategoryChange={(e) => changeFilter(e.target.value)}
                        locations={locations}
                    />
                </div>
                <div className="filter-group">
                    <SearchBar 
                        onSearch={setSearch}
                        initialValue={searchTerm}
                        placeholder="Search by room name or booker..."
                    />
                </div>
            </div>

            {/* Show total filtered count */}
            <div className="total-bookings">
                Showing {filteredBookings.length} of {useBookings().bookings.length} bookings
                {searchTerm && ` matching "${searchTerm}"`}
                {categoryFilter !== 'All' && ` in ${categoryFilter}`}
            </div>

            {bookingsLoading ? (
                <div className="loading-container">
                    <div className="spinner"></div>
                    <p>Loading bookings...</p>
                </div>
            ) : filteredBookings.length === 0 ? (
                <div className="no-results">
                    <h3>No bookings found</h3>
                    <p>Try adjusting your filters or create a new booking</p>
                    <Link href="/dashboard" className="btn btn-primary" style={{ marginTop: '1rem' }}>
                        Create Booking
                    </Link>
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
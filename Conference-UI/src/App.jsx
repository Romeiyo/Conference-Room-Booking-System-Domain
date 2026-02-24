
import React from 'react';
import { useState, useEffect } from 'react';
import Navbar from './components/Navbar';
import Footer from './components/footer';
import BookingList from './components/BookingList';
import Button from './components/Button';
import BookingForm from './components/BookingForm';
import Filter from './components/Filter';
import { useBookings } from './hooks/useBookings';
import './App.css'

function App() {
  const {
    // State
    filteredBookings,
    isLoading,
    error,
    selectedBookingId,
    categoryFilter,
    locations,
    
    // Stats
    totalBookings,
    
    // Actions
    addBooking,
    selectBooking,
    cancelSelectedBooking,
    changeFilter,
    retryFetch,
    refreshBookings,
  } = useBookings();

  // Set document title with booking count
  useEffect(() => {
    document.title = `Bookings (${totalBookings})`;
  }, [totalBookings]);

  // Show error message if present
  if (error && !isLoading && filteredBookings.length === 0) {
    return (
      <div className='app-container'>
        <Navbar />
        <main className='main-content'>
          <div className="error-container" style={{ 
            textAlign: 'center', 
            padding: '3rem',
            background: '#ffebee',
            borderRadius: '8px',
            color: '#c62828',
            margin: '2rem auto',
            maxWidth: '600px'
          }}>
            <h2>⚠️ Oops! Something went wrong</h2>
            <p>{error}</p>
            <p>The system couldn't load your bookings.</p>
            <div style={{ marginTop: '1.5rem', display: 'flex', gap: '1rem', justifyContent: 'center' }}>
              <Button label="Retry Loading" onClick={retryFetch} />
              <Button label="Refresh" onClick={refreshBookings} />
            </div>
          </div>
        </main>
        <Footer />
      </div>
    );
  }

  // Loading state
  if (isLoading && filteredBookings.length === 0) {
    return (
      <div className='app-container'>
        <Navbar />
        <main className='main-content'>
          <div className="loading-container" style={{ 
            textAlign: 'center', 
            padding: '3rem'
          }}>
            <h2>Loading Bookings...</h2>
            <p>This might take a moment... please wait</p>
            <div className="spinner" style={{
              width: '50px',
              height: '50px',
              border: '5px solid #f3f3f3',
              borderTop: '5px solid #667eea',
              borderRadius: '50%',
              animation: 'spin 1s linear infinite',
              margin: '2rem auto'
            }}></div>
          </div>
        </main>
        <Footer />
      </div>
    );
  }

  return (
    <div className='app-container'>
      <Navbar />
      <main className='main-content'>
        {/* Error banner (non-critical errors) */}
        {error && (
          <div className="error-banner" style={{
            background: '#fff3cd',
            color: '#856404',
            padding: '0.75rem 1rem',
            borderRadius: '4px',
            marginBottom: '1rem',
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center'
          }}>
            <span>⚠️ {error}</span>
            <button 
              onClick={retryFetch}
              style={{
                background: 'none',
                border: 'none',
                color: '#856404',
                textDecoration: 'underline',
                cursor: 'pointer'
              }}
            >
              Retry
            </button>
          </div>
        )}

        {/* Total Bookings counter */}
        <div className="total-bookings">
          Total Bookings: {totalBookings}
        </div>

        {/* Split layout */}
        <div className="dashboard-layout">
          {/* Left Column - Fixed Form */}
          <div className="form-column">
            <BookingForm onAddBooking={addBooking} />
            
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
                onClick={refreshBookings}
              />
            </div>
          </div>

          {/* Right Column - Scrollable Bookings */}
          <div className="bookings-column">
            {/* Filter */}
            <Filter 
              categoryFilter={categoryFilter}
              onCategoryChange={(e) => changeFilter(e.target.value)}
              locations={locations}
            />

            {isLoading && (
              <p className="loading-text">Refreshing data...</p>
            )}
            
            {selectedBookingId && (
              <p className='selected-info'>
                Selected Booking ID: {selectedBookingId}
              </p>
            )}
            
            <BookingList 
              bookings={filteredBookings}
              selectedBookingId={selectedBookingId}
              onSelectBooking={selectBooking}
            />
            
            {!isLoading && filteredBookings.length === 0 && (
              <div className="no-results" style={{
                textAlign: 'center',
                padding: '3rem',
                background: '#f9f9f9',
                borderRadius: '8px',
                marginTop: '1rem'
              }}>
                <p style={{ fontSize: '1.2rem', color: '#666' }}>
                  {categoryFilter === 'All' 
                    ? 'No bookings found. Create your first booking!' 
                    : `No bookings found in ${categoryFilter}`}
                </p>
              </div>
            )}
          </div>
        </div>
      </main>
      <Footer />

      {/* Add spin animation to global styles */}
      <style>{`
        @keyframes spin {
          0% { transform: rotate(0deg); }
          100% { transform: rotate(360deg); }
        }
      `}</style>
    </div>
  );
}

export default App;
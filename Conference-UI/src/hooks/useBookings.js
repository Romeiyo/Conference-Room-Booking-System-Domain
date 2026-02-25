import { useState, useEffect, useCallback } from 'react';
import { bookingService, fetchAllBookings } from '../Services/bookingService';
import axios from 'axios';

export function useBookings() {
  const [bookings, setBookings] = useState([]);
  const [filteredBookings, setFilteredBookings] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedBookingId, setSelectedBookingId] = useState(null);
  const [categoryFilter, setCategoryFilter] = useState('All');
  const [retryCount, setRetryCount] = useState(0);

  // Fetch bookings from API
  const fetchBookings = useCallback(async (signal) => {
    try {
      setIsLoading(true);
      setError(null);
      
      // Try to get from real API first
      try {
        const data = await bookingService.getBookings({signal});
        setBookings(data);
      } catch (apiError) {

        // Check if this was a cancellation
        if (axios.isCancel(apiError)) {
          console.log('Fetch cancelled:', apiError.message);
          return; // Silently ignore cancellations
        }
        
        // Handle specific error types
        if (apiError.code === 'ECONNABORTED') {
          throw new Error('Request timeout - server took too long to respond. Please try again.');
        }
        
        if (apiError.message === 'Network Error') {
          throw new Error('Network error - unable to reach the server. Please check your connection.');
        }
        
        if (apiError.response) {
          // Server responded with error status
          throw new Error(`Server error (${apiError.response.status}): ${apiError.response.data?.message || 'Something went wrong'}`);
        }

        console.warn('API failed, falling back to mock data:', apiError);
        
        // Fallback to mock data simulation
        const mockData = await fetchAllBookings();
        setBookings(mockData);
      }
      
    } catch (err) {
      console.error('Failed to fetch bookings:', err);
      setError(err.message || 'Failed to load bookings');
      
      // Try to load from localStorage as last resort
      const savedBookings = localStorage.getItem('bookings');
      if (savedBookings) {
        setBookings(JSON.parse(savedBookings));
      }
    } finally {
      setIsLoading(false);
    }
  }, []);

  // Initial fetch
  useEffect(() => {
    const controller = new AbortController();
    const {signal} = controller;
    fetchBookings(signal);

    return() => {
        controller.abort('Component unmounted - request cancelled');
    }
  }, [fetchBookings, retryCount]);

  // Filter bookings when category or bookings change
  useEffect(() => {
    if (categoryFilter === 'All') {
      setFilteredBookings(bookings);
    } else {
      const filtered = bookings.filter(
        booking => booking.location === categoryFilter
      );
      setFilteredBookings(filtered);
    }
  }, [categoryFilter, bookings]);

  // Save to localStorage for persistence
  useEffect(() => {
    if (bookings.length > 0) {
      localStorage.setItem('bookings', JSON.stringify(bookings));
    }
  }, [bookings]);

  // Add new booking
  const addBooking = async (newBooking) => {
    try {
      setError(null);
      
      // Try to create via API
      let createdBooking;
      try {
        createdBooking = await bookingService.createBooking({
          ...newBooking,
          status: 'Confirmed'
        });
      } catch (apiError) {
         // Handle specific API errors
        if (axios.isCancel(apiError)) {
          return; // Silently ignore cancellations
        }
        
        if (apiError.code === 'ECONNABORTED') {
          throw new Error('Request timeout - server took too long to respond. Please try again.');
        }
        
        if (apiError.message === 'Network Error') {
          throw new Error('Network error - unable to reach the server. Please check your connection.');
        }
        
        if (apiError.response?.status === 409) {
          throw new Error('Booking conflict - this time slot is already taken.');
        }

        console.warn('API create failed, using local creation:', apiError);
        
        // Fallback: create locally
        createdBooking = {
          id: Date.now(),
          ...newBooking,
          status: 'Confirmed'
        };
      }
      
      setBookings(prev => [...prev, createdBooking]);
      return createdBooking;
      
    } catch (err) {
      console.error('Failed to add booking:', err);
      setError(err.message || 'Failed to add booking');
      throw err;
    }
  };

  // Select a booking
  const selectBooking = (bookingId) => {
    setSelectedBookingId(bookingId);
  };

  // Cancel selected booking
  const cancelSelectedBooking = async () => {
    if (!selectedBookingId) {
      alert("Please select a booking to cancel");
      return false;
    }

    if (window.confirm('Are you sure you want to cancel this selected booking?')) {
      try {
        setError(null);
        
        // Try to cancel via API
        try {
          await bookingService.cancelBooking?.(selectedBookingId);
        } catch (apiError) {
            // Handle specific API errors
          if (axios.isCancel(apiError)) {
            return false;
          }
          
          if (apiError.code === 'ECONNABORTED') {
            throw new Error('Request timeout - server took too long to respond.');
          }
          
          if (apiError.message === 'Network Error') {
            throw new Error('Network error - unable to reach the server.');
          }

          console.warn('API cancel failed, using local cancellation:', apiError);
        }
        
        // Update local state
        setBookings(prev => prev.filter(booking => booking.id !== selectedBookingId));
        setSelectedBookingId(null);
        return true;
        
      } catch (err) {
        console.error('Failed to cancel booking:', err);
        setError(err.message || 'Failed to cancel booking');
        return false;
      }
    }
    return false;
  };

  // Cancel specific booking by ID
  const cancelBooking = async (bookingId) => {
    if (!bookingId) return false;

    if (window.confirm('Are you sure you want to cancel this booking?')) {
      try {
        setError(null);
        
        // Try to cancel via API
        try {
          await bookingService.cancelBooking?.(bookingId);
        } catch (apiError) {
            // Handle specific API errors
          if (axios.isCancel(apiError)) {
            return false;
          }
          
          if (apiError.code === 'ECONNABORTED') {
            throw new Error('Request timeout - server took too long to respond.');
          }
          
          if (apiError.message === 'Network Error') {
            throw new Error('Network error - unable to reach the server.');
          }
          
          console.warn('API cancel failed, using local cancellation:', apiError);
        }
        
        // Update local state
        setBookings(prev => prev.filter(booking => booking.id !== bookingId));
        
        // Clear selected if it was the cancelled one
        if (selectedBookingId === bookingId) {
          setSelectedBookingId(null);
        }
        
        return true;
        
      } catch (err) {
        console.error('Failed to cancel booking:', err);
        setError(err.message || 'Failed to cancel booking');
        return false;
      }
    }
    return false;
  };

  // Change filter
  const changeFilter = (filterValue) => {
    setCategoryFilter(filterValue);
  };

  // Retry fetching
  const retryFetch = () => {
    setRetryCount(prev => prev + 1);
  };

  // Refresh bookings
  const refreshBookings = () => {
    fetchBookings();
  };

  // Get unique locations for filter
  const locations = ['All', ...new Set(bookings.map(b => b.location))];

  // Calculate stats
  const totalBookings = bookings.length;
  const confirmedBookings = bookings.filter(b => b.status === 'Confirmed').length;
  const cancelledBookings = bookings.filter(b => b.status === 'Cancelled').length;
  const pendingBookings = bookings.filter(b => b.status === 'Pending' || !b.status).length;

  return {
    // State
    bookings,
    filteredBookings,
    isLoading,
    error,
    selectedBookingId,
    categoryFilter,
    locations,
    
    // Stats
    totalBookings,
    confirmedBookings,
    cancelledBookings,
    pendingBookings,
    
    // Actions
    addBooking,
    selectBooking,
    cancelSelectedBooking,
    cancelBooking,
    changeFilter,
    retryFetch,
    refreshBookings,
    
    // Derived
    hasBookings: bookings.length > 0,
    hasFilteredBookings: filteredBookings.length > 0,
  };
}

export default useBookings;
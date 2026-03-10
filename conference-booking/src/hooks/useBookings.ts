'use client';

import { useState, useEffect } from 'react';
import { bookingService } from '../services/bookingService';
import axios from 'axios';

export function useBookings() {
  const [bookings, setBookings] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const [filter, setFilter] = useState('All');
  const [formErrors, setFormErrors] = useState({});

  useEffect(() => {
    fetchBookings();
  }, []);

  const fetchBookings = async () => {
    try {
      setLoading(true);
      const data = await bookingService.getBookings();
      setBookings(data);
    } catch (err) {
      setError('Failed to load bookings');
    } finally {
      setLoading(false);
    }
  };

  const addBooking = async (newBooking: any) => {
    try {
      setFormErrors({});
      const result = await bookingService.createBooking(newBooking);
      await fetchBookings();
      return { success: true, data: result };
    } catch (err: any) {
      if (err.response?.status === 409) {
        setFormErrors({ time: 'This time slot is already booked' });
      } else if (err.response?.status === 400 && err.response.data?.errors) {
        const errors: any = {};
        const fieldMap: any = {
          'StartTime': 'startTime',
          'EndTime': 'endTime',
          'Room': 'roomName',
          'BookingDate': 'date',
          'BookedBy': 'bookedBy'
        };
        
        Object.keys(err.response.data.errors).forEach(key => {
          const field = fieldMap[key] || key.toLowerCase();
          errors[field] = err.response.data.errors[key].join(', ');
        });
        setFormErrors(errors);
      }
      return { success: false, error: err };
    }
  };

  const cancelSelectedBooking = async () => {
    if (!selectedId) return false;
    if (!confirm('Cancel this booking?')) return false;
    
    try {
      await bookingService.cancelBooking(selectedId);
      setBookings(prev => prev.filter(b => b.id !== selectedId));
      setSelectedId(null);
      return true;
    } catch (err) {
      setError('Failed to cancel booking');
      return false;
    }
  };

  const filteredBookings = filter === 'All' 
    ? bookings 
    : bookings.filter(b => b.location === filter);

  const locations = ['All', ...new Set(bookings.map(b => b.location))];

  return {
    bookings: filteredBookings,
    loading,
    error,
    selectedId,
    filter,
    locations,
    formErrors,
    setSelectedId,
    setFilter,
    addBooking,
    cancelSelectedBooking,
    refresh: fetchBookings
  };
}
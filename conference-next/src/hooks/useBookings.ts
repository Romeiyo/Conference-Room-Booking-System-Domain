"use client";

import { useState, useEffect, useCallback } from 'react';
import { bookingService, fetchAllBookings } from '@/services/bookingService';
import axios from 'axios';

interface Booking {
    id: number;
    roomName?: string;
    room?: { name: string; location: string };
    location?: string;
    bookingDate?: string;
    date?: string;
    startTime?: string;
    endTime?: string;
    status: string;
    bookedBy?: string;
}

export function useBookings() {
    const [bookings, setBookings] = useState<Booking[]>([]);
    const [filteredBookings, setFilteredBookings] = useState<Booking[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [selectedBookingId, setSelectedBookingId] = useState<number | null>(null);
    const [categoryFilter, setCategoryFilter] = useState('All');
    const [retryCount, setRetryCount] = useState(0);
    const [formErrors, setFormErrors] = useState<Record<string, string>>({});

    const fetchBookings = useCallback(async (signal?: AbortSignal) => {
        try {
            setIsLoading(true);
            setError(null);
            
            try {
                const data = await bookingService.getBookings({ signal });
                setBookings(data);
            } catch (apiError: any) {
                if (axios.isCancel(apiError)) {
                    console.log('Fetch cancelled:', apiError.message);
                    return;
                }
                
                if (apiError.code === 'ECONNABORTED') {
                    throw new Error('Request timeout - server took too long to respond. Please try again.');
                }
                
                if (apiError.message === 'Network Error') {
                    throw new Error('Network error - unable to reach the server. Please check your connection.');
                }
                
                if (apiError.response) {
                    if (apiError.response.data && apiError.response.data.errors) {
                        const problemDetails = apiError.response.data;
                        throw {
                            message: problemDetails.title || 'Validation error',
                            errors: problemDetails.errors,
                            status: apiError.response.status
                        };
                    }
                    
                    throw new Error(`Server error (${apiError.response.status}): ${apiError.response.data?.message || 'Something went wrong'}`);
                }

                console.warn('API failed, falling back to mock data:', apiError);
                
                const mockData = await fetchAllBookings();
                setBookings(mockData);
            }
            
        } catch (err: any) {
            console.error('Failed to fetch bookings:', err);
            
            if (err.errors) {
                setError(err.message);
                setFormErrors(err.errors);
            } else {
                setError(err.message || 'Failed to load bookings');
            }
            
            const savedBookings = localStorage.getItem('bookings');
            if (savedBookings) {
                setBookings(JSON.parse(savedBookings));
            }
        } finally {
            setIsLoading(false);
        }
    }, []);

    useEffect(() => {
        const controller = new AbortController();
        const { signal } = controller;
        fetchBookings(signal);

        return () => {
            controller.abort('Component unmounted - request cancelled');
        };
    }, [fetchBookings, retryCount]);

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

    useEffect(() => {
        if (bookings.length > 0) {
            localStorage.setItem('bookings', JSON.stringify(bookings));
        }
    }, [bookings]);

    const addBooking = async (newBooking: any) => {
        try {
            setError(null);
            setFormErrors({});

            try {
                const createdBooking = await bookingService.createBooking({
                    ...newBooking,
                    status: 'Booked'
                });

                setBookings(prev => [...prev, createdBooking]);
                return { success: true, data: createdBooking };

            } catch (apiError: any) {
                if (axios.isCancel(apiError)) {
                    return { success: false, cancelled: true };
                }

                if (apiError.code === 'ECONNABORTED') {
                    throw new Error('Request timeout - server took too long to respond. Please try again.');
                }

                if (apiError.message === 'Network Error') {
                    throw new Error('Network error - unable to reach the server. Please check your connection.');
                }

                if (apiError.response && apiError.response.status === 409) {
                    const fieldError = {
                        time: 'This time slot is already booked. Please choose a different time.'
                    };
                    setFormErrors(fieldError);
                    throw {
                        message: 'Booking conflict',
                        errors: fieldError
                    };
                }

                if (apiError.response && apiError.response.status === 400) {
                    const responseData = apiError.response.data;

                    console.log('🔍 FULL ERROR RESPONSE:', apiError.response);
                    console.log('🔍 ERROR DATA:', responseData);

                    if (responseData && responseData.errors) {
                        console.log('🔍 VALIDATION ERRORS:', responseData.errors);
                        const fieldErrors: Record<string, string> = {};
                        Object.keys(responseData.errors).forEach(key => {
                            const fieldMap: Record<string, string> = {
                                'StartTime': 'startTime',
                                'EndTime': 'endTime',
                                'Room': 'roomName',
                                'BookingDate': 'date',
                                'BookedBy': 'bookedBy'
                            };

                            const frontendField = fieldMap[key] || key.toLowerCase();
                            fieldErrors[frontendField] = responseData.errors[key].join(', ');
                        });

                        setFormErrors(fieldErrors);
                        throw {
                            message: responseData.title || 'Validation error',
                            errors: fieldErrors
                        };
                    }
                }

                if (apiError.response) {
                    const status = apiError.response.status;
                    const responseData = apiError.response.data;

                    if (status === 422) {
                        throw new Error(responseData?.detail || responseData?.title || 'Invalid booking data');
                    }

                    throw new Error(`Server error (${status}): ${responseData?.message || 'Something went wrong'}`);
                }

                console.warn('API create failed, using local creation:', apiError);

                const createdBooking = {
                    id: Date.now(),
                    ...newBooking,
                    status: 'Booked'
                };

                setBookings(prev => [...prev, createdBooking]);
                return { success: true, data: createdBooking, fallback: true };
            }

        } catch (err: any) {
            console.error('Failed to add booking:', err);

            if (err.errors) {
                setFormErrors(err.errors);
                setError(err.message);
            } else {
                setError(err.message || 'Failed to add booking');
            }

            return { success: false, error: err };
        }
    };

    const selectBooking = (bookingId: number) => {
        setSelectedBookingId(bookingId);
    };

    const cancelSelectedBooking = async () => {
        if (!selectedBookingId) {
            alert("Please select a booking to cancel");
            return false;
        }

        if (window.confirm('Are you sure you want to cancel this selected booking?')) {
            try {
                setError(null);
                
                try {
                    await bookingService.cancelBooking?.(selectedBookingId);
                } catch (apiError: any) {
                    if (axios.isCancel(apiError)) {
                        return false;
                    }
                    
                    if (apiError.code === 'ECONNABORTED') {
                        throw new Error('Request timeout - server took too long to respond.');
                    }
                    
                    if (apiError.message === 'Network Error') {
                        throw new Error('Network error - unable to reach the server.');
                    }

                    if (apiError.response?.data && apiError.response.data.errors) {
                        throw {
                            message: apiError.response.data.title || 'Cancellation failed',
                            errors: apiError.response.data.errors
                        };
                    }

                    console.warn('API cancel failed, using local cancellation:', apiError);
                }
                
                setBookings(prev => prev.filter(booking => booking.id !== selectedBookingId));
                setSelectedBookingId(null);
                return true;
                
            } catch (err: any) {
                console.error('Failed to cancel booking:', err);
                
                if (err.errors) {
                    setFormErrors(err.errors);
                    setError(err.message);
                } else {
                    setError(err.message || 'Failed to cancel booking');
                }
                
                return false;
            }
        }
        return false;
    };

    const cancelBooking = async (bookingId: number) => {
        if (!bookingId) return false;

        if (window.confirm('Are you sure you want to cancel this booking?')) {
            try {
                setError(null);
                
                try {
                    await bookingService.cancelBooking?.(bookingId);
                } catch (apiError: any) {
                    if (axios.isCancel(apiError)) {
                        return false;
                    }
                    
                    if (apiError.code === 'ECONNABORTED') {
                        throw new Error('Request timeout - server took too long to respond.');
                    }
                    
                    if (apiError.message === 'Network Error') {
                        throw new Error('Network error - unable to reach the server.');
                    }

                    if (apiError.response?.data && apiError.response.data.errors) {
                        throw {
                            message: apiError.response.data.title || 'Cancellation failed',
                            errors: apiError.response.data.errors
                        };
                    }
                    
                    console.warn('API cancel failed, using local cancellation:', apiError);
                }
                
                setBookings(prev => prev.filter(booking => booking.id !== bookingId));
                
                if (selectedBookingId === bookingId) {
                    setSelectedBookingId(null);
                }
                
                return true;
                
            } catch (err: any) {
                console.error('Failed to cancel booking:', err);
                
                if (err.errors) {
                    setFormErrors(err.errors);
                    setError(err.message);
                } else {
                    setError(err.message || 'Failed to cancel booking');
                }
                
                return false;
            }
        }
        return false;
    };

    const changeFilter = (filterValue: string) => {
        setCategoryFilter(filterValue);
    };

    const retryFetch = () => {
        setRetryCount(prev => prev + 1);
    };

    const refreshBookings = () => {
        fetchBookings();
    };

    const clearFormErrors = () => {
        setFormErrors({});
    };

    const locations = ['All', ...new Set(bookings.map(b => b.location).filter(Boolean))] as string[];

    const totalBookings = bookings.length;
    const confirmedBookings = bookings.filter(b => b.status === 'Confirmed').length;
    const cancelledBookings = bookings.filter(b => b.status === 'Cancelled').length;
    const pendingBookings = bookings.filter(b => b.status === 'Pending' || !b.status).length;

    return {
        bookings,
        filteredBookings,
        isLoading,
        error,
        formErrors,
        selectedBookingId,
        categoryFilter,
        locations,
        
        totalBookings,
        confirmedBookings,
        cancelledBookings,
        pendingBookings,
        
        addBooking,
        selectBooking,
        cancelSelectedBooking,
        cancelBooking,
        changeFilter,
        retryFetch,
        refreshBookings,
        clearFormErrors,
        
        hasBookings: bookings.length > 0,
        hasFilteredBookings: filteredBookings.length > 0,
    };
}

export default useBookings;
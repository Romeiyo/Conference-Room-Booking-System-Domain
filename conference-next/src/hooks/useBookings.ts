"use client";

import { useState, useEffect, useCallback, useMemo } from 'react';
import { bookingService, fetchAllBookings } from '@/services/bookingService';
import axios from 'axios';
import { useDebounce } from '@/hooks/useDebounce';

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
    const [searchInput, setSearchInput] = useState(''); 

    const debouncedSearchTerm = useDebounce(searchInput, 400);

    // These only recalculate when bookings array changes
    const bookingStats = useMemo(() => {
        console.log('Calculating booking statistics...');
        return {
            total: bookings.length,
            confirmed: bookings.filter(b => b.status === 'Confirmed').length,
            cancelled: bookings.filter(b => b.status === 'Cancelled').length,
            pending: bookings.filter(b => b.status === 'Pending' || !b.status).length,
            uniqueLocations: [...new Set(bookings.map(b => b.location).filter(Boolean))] as string[]
        };
    }, [bookings]);

    const locations = useMemo(() => {
        return ['All', ...bookingStats.uniqueLocations];
    }, [bookingStats.uniqueLocations]); //Only recalculates when dropdown changes

    // This prevents re-filtering on every render
    const getFilteredBookings = useMemo(() => {
        console.log('🔍 Filtering bookings...', { 
            searchTerm: debouncedSearchTerm, // This is the debounced value
            categoryFilter 
        });

        let result = [...bookings];
        
        // Apply category filter
        if (categoryFilter !== 'All') {
            result = result.filter(booking => booking.location === categoryFilter);
        }
        
        // Apply search filter (if searchTerm exists)
        if (debouncedSearchTerm.trim()) {
            const term = debouncedSearchTerm.toLowerCase();
            result = result.filter(booking => 
                booking.roomName?.toLowerCase().includes(term) ||
                booking.room?.name?.toLowerCase().includes(term) ||
                booking.bookedBy?.toLowerCase().includes(term)
            );
        }
        
        return result;
    }, [bookings, categoryFilter, debouncedSearchTerm]);

    // Update filteredBookings when getFilteredBookings changes
    useEffect(() => {
        setFilteredBookings(getFilteredBookings);
    }, [getFilteredBookings]);

    // These prevent child components from re-rendering unnecessarily
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

    const selectBooking = useCallback((bookingId: number) => {
        setSelectedBookingId(bookingId);
    }, []); 

    const changeFilter = useCallback((filterValue: string) => {
        setCategoryFilter(filterValue);
    }, []);

    const setSearch = useCallback((term: string) => {
        console.log('Search input changed:', term); // Wendy's Keylogger

        setSearchInput(term);
    }, []); 

    const retryFetch = useCallback(() => {
        setRetryCount(prev => prev + 1);
    }, []);

    const clearFormErrors = useCallback(() => {
        setFormErrors({});
    }, []);

    // Fetch effect
    useEffect(() => {
        const controller = new AbortController();
        const { signal } = controller;
        fetchBookings(signal);

        return () => {
            controller.abort('Component unmounted - request cancelled');
        };
    }, [fetchBookings, retryCount]);

    // Save to localStorage
    useEffect(() => {
        if (bookings.length > 0) {
            localStorage.setItem('bookings', JSON.stringify(bookings));
        }
    }, [bookings]);

    // Add booking function
    const addBooking = useCallback(async (newBooking: any) => {
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

                    if (responseData && responseData.errors) {
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
    }, []);

    const cancelSelectedBooking = useCallback(async () => {
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
    }, [selectedBookingId]);

    const cancelBooking = useCallback(async (bookingId: number) => {
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
    }, [selectedBookingId]);

    const refreshBookings = useCallback(() => {
        fetchBookings();
    }, [fetchBookings]);

    return {
        bookings,
        filteredBookings,
        isLoading,
        error,
        formErrors,
        selectedBookingId,
        categoryFilter,
        locations,
        
        totalBookings: bookingStats.total,
        confirmedBookings: bookingStats.confirmed,
        cancelledBookings: bookingStats.cancelled,
        pendingBookings: bookingStats.pending,
        
        addBooking,
        selectBooking,
        cancelSelectedBooking,
        cancelBooking,
        changeFilter,
        retryFetch,
        refreshBookings,
        clearFormErrors,
        
        searchTerm: searchInput,
        debouncedSearchTerm,
        setSearch,
        
        hasBookings: bookings.length > 0,
        hasFilteredBookings: filteredBookings.length > 0,
    };
}

export default useBookings;
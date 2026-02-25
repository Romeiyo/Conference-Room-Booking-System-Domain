import apiClient from "./api";
import initialBookings from "../mockData";

export const fetchAllBookings = () => {
    return new Promise((resolve, reject) => {
    
    const delay = Math.random() * 2000 + 500;

    const shouldFail = Math.random() < 0.2;

    setTimeout(() => {
        if(shouldFail){
            reject(new Error('Failed to fetch bookings'));
        }
        else{
            resolve(initialBookings);
        }
    }, delay);
  });
};

export const bookingService = {
    // Get bookings from real API
    async getBookings() {
        try {
            const response = await apiClient.get('/booking/bookings/roomName');
            if (response.data && response.data.data) {
                return response.data.data;
            }
            else if (Array.isArray(response.data)) {
                return response.data;
            }
            else {
                console.warn('Unexpected API response format:', response.data);
                return [];
            }
        } catch (error) {
            console.error('Error fetching bookings:', error);
            throw error;
        }
    },

    // Get booking by ID
    async getBookingById(bookingId) {
        try {
            const response = await apiClient.get(`/bookings/${bookingId}`);
            return response.data;
        } catch (error) {
            console.error('Error fetching booking:', error);
            throw error;
        }
    },

    // Get rooms from real API
    async getRooms() {
        try {
            const response = await apiClient.get('/rooms');
            return response.data;
        } catch (error) {
            console.error('Error fetching rooms:', error);
            throw error;
        }
    },

    // Create new booking in real API
    async createBooking(bookingData) {
        try {
            const response = await apiClient.post('/bookings', bookingData);
            return response.data;
        } catch (error) {
            console.error('Error creating booking:', error);
            throw error;
        }
    },

    // Cancel a booking
    async cancelBooking(bookingId) {
        try {
            const response = await apiClient.delete(`/bookings/${bookingId}`);
            return response.data;
        } catch (error) {
            console.error('Error cancelling booking:', error);
            throw error;
        }
    },

    // Get cancelled bookings from real API
    async getCancelledBooking() {
        try {
            const response = await apiClient.get('/bookings/status/Cancelled');
            return response.data;
        } catch (error) {
            console.error('Error fetching cancelled bookings:', error);
            throw error;
        }
    },

    // Get bookings by location
    async getBookingsByLocation(location, page = 1, pageSize = 10) {
        try {
            const response = await apiClient.get(`/bookings/location/${location}`, {
                params: { page, pageSize }
            });
            return response.data;
        } catch (error) {
            console.error('Error fetching bookings by location:', error);
            throw error;
        }
    },

    // Get bookings by date
    async getBookingsByDate(year, month, day) {
        try {
            const response = await apiClient.get(`/bookings/date/${year}/${month}/${day}`);
            return response.data;
        } catch (error) {
            console.error('Error fetching bookings by date:', error);
            throw error;
        }
    }
};
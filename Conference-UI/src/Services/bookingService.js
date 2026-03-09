import apiClient from "../api/apiClient";
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
            
            const data = await apiClient.get('/booking/bookings/roomName');
            
            if (data && data.data) {
                return data.data;
            }
            else if (Array.isArray(data)) {
                return data;
            }
            else {
                console.warn('Unexpected API response format:', data);
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
            return apiClient.get(`/bookings/${bookingId}`);
        } catch (error) {
            console.error('Error fetching booking:', error);
            throw error;
        }
    },


    // Create new booking in real API
    async createBooking(bookingData) {
        try {
            // Transforming the booking data to match API expectations
            // Map room name to room ID
            const roomId = getRoomIdFromName(bookingData.roomName);
            
            const formatTimeWithSeconds = (timeString) => {
                if (timeString.length === 5) { // "21:36" format
                    return `${timeString}:00`; // Add seconds
                }
                return timeString;
            };

            const apiBookingData = {
                room: { 
                    id: roomId 
                },
                bookingDate: bookingData.date,
                startTime: formatTimeWithSeconds(bookingData.startTime),
                endTime: formatTimeWithSeconds(bookingData.endTime)
            };

            
            
            // 🔍 ADD DETAILED LOGGING
            console.log('📤 Sending to API:', {
                url: '/booking',
                method: 'POST',
                data: apiBookingData,
                roomId: roomId,
                roomName: bookingData.roomName,
                date: bookingData.date,
                startTime: bookingData.startTime,
                endTime: bookingData.endTime
            });
        
            const response = await apiClient.post('/booking', apiBookingData);
            console.log('Booking response:', response);

            return response;
        } catch (error) {
            console.error('Error creating booking:', error);
            throw error;
        }
    },

    // Cancel a booking
    async cancelBooking(bookingId) {
        try {
            return await apiClient.delete(`/bookings/${bookingId}`);
        } catch (error) {
            console.error('Error cancelling booking:', error);
            throw error;
        }
    },

    // Get cancelled bookings from real API
    async getCancelledBooking() {
        try {
            const data = await apiClient.get('/bookings/status/Cancelled');
            return data;
        } catch (error) {
            console.error('Error fetching cancelled bookings:', error);
            throw error;
        }
    },

    // Get bookings by location
    async getBookingsByLocation(location, page = 1, pageSize = 10) {
        try {
            return await apiClient.get(`/bookings/location/${location}`, {
                params: { page, pageSize }
            });
        } catch (error) {
            console.error('Error fetching bookings by location:', error);
            throw error;
        }
    },

    // Get bookings by date
    async getBookingsByDate(year, month, day) {
        try {
            return await apiClient.get(`/bookings/date/${year}/${month}/${day}`);
        } catch (error) {
            console.error('Error fetching bookings by date:', error);
            throw error;
        }
    },

    // Get bookings by room type
    async getBookingsByRoomType(roomType, page = 1, pageSize = 10) {
        try {
            return await apiClient.get(`/booking/bookings/roomType/${roomType}`, {
                params: { page, pageSize }
            });
        } catch (error) {
            console.error('Error fetching bookings by room type:', error);
            throw error;
        }
    },

    // Get bookings by status
    async getBookingsByStatus(status, page = 1, pageSize = 10) {
        try {
            return await apiClient.get(`/booking/bookings/status/${status}`, {
                params: { page, pageSize }
            });
        } catch (error) {
            console.error('Error fetching bookings by status:', error);
            throw error;
        }
    },

    // Get my bookings (for current user)
    async getMyBookings(page = 1, pageSize = 10) {
        try {
            return await apiClient.get('/booking/my-bookings', {
                params: { page, pageSize }
            });
        } catch (error) {
            console.error('Error fetching my bookings:', error);
            throw error;
        }
    }
};

// Helper function to map room name to room ID
function getRoomIdFromName(roomName) {
    const roomMap = {
        'Room A': 1,
        'Room B': 2,
        'Room C': 3,
        'Room D': 4,
        'Room E': 5,
        'Room F': 6,
        'Room G': 7,
        'Room H': 8,
        'Room I': 9,
        'Room J': 10,
        'Room K': 11,
        'Room L': 12,
        'Room M': 13,
        'Room N': 14,
        'Room O': 15,
        'Room P': 16
    };
    
    return roomMap[roomName] || 1; // Default to Room A if a room doesnt exist
}
import apiClient from "@/api/apiClient";
import initialBookings from "@/mockData";

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

interface PaginatedResponse<T> {
    totalCount: number;
    page: number;
    pageSize: number;
    sortBy: string;
    data: T[];
}

export const fetchAllBookings = (): Promise<any[]> => {
    return new Promise((resolve, reject) => {
        const delay = Math.random() * 2000 + 500;
        const shouldFail = Math.random() < 0.2;

        setTimeout(() => {
            if (shouldFail) {
                reject(new Error('Failed to fetch bookings'));
            } else {
                resolve(initialBookings);
            }
        }, delay);
    });
};

interface GetBookingsOptions {
    signal?: AbortSignal;
}

export const bookingService = {
    async getBookings(options?: GetBookingsOptions): Promise<Booking[]> {
        try {
            const response = await apiClient.get('/booking/bookings/roomName', {
                signal: options?.signal
            });
            
           if (response && typeof response === 'object') {
                // If it has a 'data' property that is an array, extract it
                if ('data' in response && Array.isArray(response.data)) {
                    console.log(`Received ${response.data.length} bookings from API`);
                    return response.data as Booking[];
                }
                // If the response itself is an array, return it directly
                else if (Array.isArray(response)) {
                    return response as Booking[];
                }
            }

            console.warn('Unexpected API response format:', response);
            return [];
        } catch (error) {
            console.error('Error fetching bookings:', error);
            throw error;
        }
    },

    async getBookingById(bookingId: number) {
        try {
            return apiClient.get(`/bookings/${bookingId}`);
        } catch (error) {
            console.error('Error fetching booking:', error);
            throw error;
        }
    },

    async createBooking(bookingData: any): Promise<Booking> {
        try {
            const roomId = getRoomIdFromName(bookingData.roomName);
            
            const formatTimeWithSeconds = (timeString: string) => {
                if (timeString.length === 5) {
                    return `${timeString}:00`;
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
            
            console.log('Sending to API:', {
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

            // The apiClient interceptor already extracts response.data
            // Map API response fields to Booking interface if needed
            const booking = response as Booking;
            
            return {
                ...booking,
                roomName: bookingData.roomName,
                date: booking.bookingDate || booking.date,
                status: booking.status || 'Booked'
            } as Booking;
        } catch (error) {
            console.error('Error creating booking:', error);
            throw error;
        }
    },

    async cancelBooking(bookingId: number) {
        try {
            return await apiClient.delete(`/bookings/${bookingId}`);
        } catch (error) {
            console.error('Error cancelling booking:', error);
            throw error;
        }
    },

    async getCancelledBooking() {
        try {
            const data = await apiClient.get('/bookings/status/Cancelled');
            return data;
        } catch (error) {
            console.error('Error fetching cancelled bookings:', error);
            throw error;
        }
    },

    async getBookingsByLocation(location: string, page = 1, pageSize = 10) {
        try {
            return await apiClient.get(`/bookings/location/${location}`, {
                params: { page, pageSize }
            });
        } catch (error) {
            console.error('Error fetching bookings by location:', error);
            throw error;
        }
    },

    async getBookingsByDate(year: number, month: number, day: number) {
        try {
            return await apiClient.get(`/bookings/date/${year}/${month}/${day}`);
        } catch (error) {
            console.error('Error fetching bookings by date:', error);
            throw error;
        }
    },

    async getBookingsByRoomType(roomType: string, page = 1, pageSize = 10) {
        try {
            return await apiClient.get(`/booking/bookings/roomType/${roomType}`, {
                params: { page, pageSize }
            });
        } catch (error) {
            console.error('Error fetching bookings by room type:', error);
            throw error;
        }
    },

    async getBookingsByStatus(status: string, page = 1, pageSize = 10) {
        try {
            return await apiClient.get(`/booking/bookings/status/${status}`, {
                params: { page, pageSize }
            });
        } catch (error) {
            console.error('Error fetching bookings by status:', error);
            throw error;
        }
    },

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

function getRoomIdFromName(roomName: string): number {
    const roomMap: Record<string, number> = {
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
    
    return roomMap[roomName] || 1;
}
import apiClient from '../api/apiClient';

const roomMap: Record<string, number> = {
  'Room A': 1, 'Room B': 2, 'Room C': 3, 'Room D': 4,
  'Room E': 5, 'Room F': 6, 'Room G': 7, 'Room H': 8,
  'Room I': 9, 'Room J': 10, 'Room K': 11, 'Room L': 12,
  'Room M': 13, 'Room N': 14, 'Room O': 15, 'Room P': 16
};

export const bookingService = {
  async getBookings() {
    const data = await apiClient.get('/booking/bookings/roomName');
    return data.data || data || [];
  },

  async createBooking(bookingData: any) {
    const roomId = roomMap[bookingData.roomName] || 1;
    
    const formatTime = (time: string) => time.length === 5 ? `${time}:00` : time;

    const payload = {
      room: { id: roomId },
      bookingDate: bookingData.date,
      startTime: formatTime(bookingData.startTime),
      endTime: formatTime(bookingData.endTime)
    };

    return await apiClient.post('/booking', payload);
  },

  async cancelBooking(bookingId: number) {
    return await apiClient.delete(`/bookings/${bookingId}`);
  }
};
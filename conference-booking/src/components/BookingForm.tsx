'use client';

import { useState } from 'react';
import Button from './Button';

const rooms = ['Room A', 'Room B', 'Room C', 'Room D', 'Room E', 'Room F', 'Room G', 'Room H',
  'Room I', 'Room J', 'Room K', 'Room L', 'Room M', 'Room N', 'Room O', 'Room P'];

export default function BookingForm({ onAddBooking, formErrors = {}, clearFormErrors }: any) {
  const [roomName, setRoomName] = useState('');
  const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
  const [startTime, setStartTime] = useState('');
  const [endTime, setEndTime] = useState('');
  const [location, setLocation] = useState('Bloemfontein');
  const [bookedBy, setBookedBy] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!roomName || !date || !startTime || !endTime || !bookedBy) {
      setError('Please fill all fields');
      return;
    }

    if (startTime >= endTime) {
      setError('End time must be after start time');
      return;
    }

    setSubmitting(true);
    const result = await onAddBooking({ roomName, date, startTime, endTime, location, bookedBy });
    setSubmitting(false);

    if (result.success) {
      setRoomName('');
      setDate(new Date().toISOString().split('T')[0]);
      setStartTime('');
      setEndTime('');
      setLocation('Bloemfontein');
      setBookedBy('');
      alert('Booking created!');
    }
  };

  const getError = (field: string) => {
    return formErrors[field] ? <span className="text-red-600 text-sm">{formErrors[field]}</span> : null;
  };

  return (
    <form onSubmit={handleSubmit} className="bg-white p-6 rounded-lg shadow space-y-4">
      <h3 className="text-xl font-bold">Book a New Room</h3>

      {error && <div className="bg-red-100 text-red-700 p-2 rounded">{error}</div>}

      <div>
        <label className="block mb-1">Room Name:</label>
        <select
          value={roomName}
          onChange={(e) => setRoomName(e.target.value)}
          className="w-full border p-2 rounded"
          disabled={submitting}
        >
          <option value="">Select a room</option>
          {rooms.map(r => <option key={r} value={r}>{r}</option>)}
        </select>
        {getError('roomName')}
      </div>

      <div>
        <label className="block mb-1">Date:</label>
        <input
          type="date"
          value={date}
          onChange={(e) => setDate(e.target.value)}
          className="w-full border p-2 rounded"
          disabled={submitting}
        />
        {getError('date')}
      </div>

      <div>
        <label className="block mb-1">Start Time:</label>
        <input
          type="time"
          value={startTime}
          onChange={(e) => setStartTime(e.target.value)}
          className="w-full border p-2 rounded"
          disabled={submitting}
        />
        {getError('startTime')}
      </div>

      <div>
        <label className="block mb-1">End Time:</label>
        <input
          type="time"
          value={endTime}
          onChange={(e) => setEndTime(e.target.value)}
          className="w-full border p-2 rounded"
          disabled={submitting}
        />
        {getError('endTime')}
      </div>

      <div>
        <label className="block mb-1">Location:</label>
        <select
          value={location}
          onChange={(e) => setLocation(e.target.value)}
          className="w-full border p-2 rounded"
          disabled={submitting}
        >
          <option value="Bloemfontein">Bloemfontein</option>
          <option value="Cape Town">Cape Town</option>
        </select>
      </div>

      <div>
        <label className="block mb-1">Booked By:</label>
        <input
          type="text"
          value={bookedBy}
          onChange={(e) => setBookedBy(e.target.value)}
          className="w-full border p-2 rounded"
          disabled={submitting}
        />
        {getError('bookedBy')}
      </div>

      {formErrors.time && <div className="text-red-600">{formErrors.time}</div>}

      <Button label={submitting ? 'Booking...' : 'Book Room'} type="submit" disabled={submitting} />
    </form>
  );
}
'use client';

import { useBookings } from '@/hooks/useBookings';
import BookingForm from './BookingForm';
import BookingList from './BookingList';
import Filter from './Filter';

export default function DashboardContent() {
  const {
    bookings,
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
    refresh
  } = useBookings();

  return (
    <div className="p-4">
      <div className="grid md:grid-cols-2 gap-6">
        <div>
          <BookingForm onAddBooking={addBooking} formErrors={formErrors} />
        </div>

        <div>
          <div className="flex justify-between items-center mb-4">
            <h1 className="text-2xl font-bold">Bookings</h1>
            {selectedId && (
              <button
                onClick={cancelSelectedBooking}
                className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
              >
                Cancel Selected
              </button>
            )}
          </div>

          {error && (
            <div className="bg-red-100 text-red-700 p-2 rounded mb-4">
              {error}
              <button onClick={refresh} className="ml-2 underline">Retry</button>
            </div>
          )}

          <Filter filter={filter} onChange={setFilter} locations={locations} />

          {loading ? (
            <div className="text-center py-8">Loading bookings...</div>
          ) : (
            <BookingList
              bookings={bookings}
              selectedId={selectedId}
              onSelect={setSelectedId}
            />
          )}
        </div>
      </div>
    </div>
  );
}
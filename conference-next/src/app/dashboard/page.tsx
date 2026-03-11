"use client";

import { useBookings } from '@/hooks/useBookings';
import { useAuth } from '@/hooks/useAuth';
import BookingForm from '@/components/BookingForm';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import '@/app/globals.css';

export default function NewBookingPage() {
    const router = useRouter();
    const {
        formErrors,
        addBooking,
        clearFormErrors
    } = useBookings();
    
    const { user } = useAuth();

    const handleAddBooking = async (bookingData: any) => {
        try {
            await addBooking(bookingData);
            alert('Booking created successfully!');
            router.push('/bookings'); 
        } catch (error) {
            console.error('Failed to create booking:', error);
            alert('Failed to create booking. Please try again.');
        }
    };

    return (
        <div className="container">
            <div className="header-section">
                <h1>Book a Conference Room</h1>
                {user && <p>Welcome, {user.username}!</p>}
                <Link href="/bookings" className="btn btn-secondary">
                    ← Back to Bookings
                </Link>
            </div>

            <div className="form-page-content">
                <BookingForm 
                    onAddBooking={handleAddBooking}
                    formErrors={formErrors}
                    clearFormErrors={clearFormErrors}
                />
            </div>
        </div>
    );
}
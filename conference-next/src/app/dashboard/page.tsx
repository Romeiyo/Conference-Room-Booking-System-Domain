"use client";

import ProtectedRoute from '@/components/ProtectedRoute';
import { useAuth } from '@/context/AuthContext';
import BookingForm from '@/components/BookingForm';
import { useBookings } from '@/hooks/useBookings';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import '@/app/globals.css';

function DashboardContent() {
    const router = useRouter();
    const { user } = useAuth();
    const {
        formErrors,
        addBooking,
        clearFormErrors
    } = useBookings();

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

export default function DashboardPage() {
    return (
        <ProtectedRoute>
            <DashboardContent />
        </ProtectedRoute>
    );
}
import { Routes, Route, Navigate } from 'react-router-dom';
import './App.css';
import Navbar from './components/Navbar';
import Footer from './components/Footer';
import BookingList from './components/BookingList';
import BookingForm from './components/BookingForm';
import Filter from './components/Filter';
import LoginForm from './components/LoginForm';
import ProtectedRoute from './components/ProtectedRoute';
import useBookings from './hooks/useBookings';
import { useAuth } from './hooks/useAuth';

// Main layout component with Navbar and Footer
function MainLayout({ children }) {
    return (
        <div className="app-container">
            <Navbar />
            <main className="main-content">
                {children}
            </main>
            <Footer />
        </div>
    );
}

// Dashboard content
function Dashboard() {
    const {
        filteredBookings,
        isLoading: bookingsLoading,
        error,
        formErrors,
        selectedBookingId,
        categoryFilter,
        locations,
        addBooking,
        selectBooking,
        cancelSelectedBooking,
        changeFilter,
        retryFetch,
        clearFormErrors
    } = useBookings();

    const handleAddBooking = async (bookingData) => {
        return await addBooking(bookingData);
    };

    const handleCancelSelected = async () => {
        const success = await cancelSelectedBooking();
        if (success) {
            alert('Booking cancelled successfully');
        }
    };

    return (
        <MainLayout>
            <div className="container">
                <div className="header-section">
                    <h1>Conference Room Bookings</h1>
                    {selectedBookingId && (
                        <button 
                            className="btn btn-danger" 
                            onClick={handleCancelSelected}
                        >
                            Cancel Selected Booking
                        </button>
                    )}
                </div>

                {error && (
                    <div className="error-container">
                        <p className="error">{error}</p>
                        <button onClick={retryFetch} className="btn btn-secondary">
                            Retry
                        </button>
                    </div>
                )}

                <div className="content-grid">
                    <div className="form-section">
                        <BookingForm 
                            onAddBooking={handleAddBooking}
                            formErrors={formErrors}
                            clearFormErrors={clearFormErrors}
                        />
                    </div>

                    <div className="list-section">
                        <Filter 
                            categoryFilter={categoryFilter}
                            onCategoryChange={(e) => changeFilter(e.target.value)}
                            locations={locations}
                        />

                        {bookingsLoading ? (
                            <div className="loading">Loading bookings...</div>
                        ) : (
                            <BookingList 
                                bookings={filteredBookings}
                                selectedBookingId={selectedBookingId}
                                onSelectBooking={selectBooking}
                            />
                        )}
                    </div>
                </div>
            </div>
        </MainLayout>
    );
}

function App() {
    const { isLoading } = useAuth();

    // Show loading while checking authentication
    if (isLoading) {
        return (
            <div style={{ 
                display: 'flex', 
                justifyContent: 'center', 
                alignItems: 'center', 
                height: '100vh' 
            }}>
                <div className="spinner"></div>
            </div>
        );
    }

    return (
        <Routes>
            <Route path="/login" element={<LoginForm />} />
            <Route 
                path="/" 
                element={
                    <ProtectedRoute>
                        <Dashboard />
                    </ProtectedRoute>
                } 
            />
            <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
    );
}

export default App;
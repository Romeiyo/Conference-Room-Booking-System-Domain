"use client";

export default function BookingsLoading() {
    return (
        <div className="container">
            <div className="header-section">
                <h1>Conference Room Bookings</h1>
                <div className="header-actions">
                    <div className="skeleton-button" style={{ width: '120px', height: '40px' }}></div>
                </div>
            </div>

            {/* Filter Section Skeleton */}
            <div className="filter-section">
                <div className="filter-group">
                    <div className="skeleton-filter" style={{ height: '80px' }}></div>
                </div>
                <div className="filter-group">
                    <div className="skeleton-search" style={{ height: '60px' }}></div>
                </div>
            </div>

            {/* Total Bookings Skeleton */}
            <div className="skeleton-total" style={{ height: '30px', width: '200px', marginBottom: '1rem' }}></div>

            {/* Booking Cards Skeleton */}
            <div className="booking-list">
                <h2>Current Bookings</h2>
                {[1, 2, 3, 4, 5].map((i) => (
                    <div key={i} className="booking-card-skeleton" style={{ marginBottom: '1rem' }}>
                        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1rem', padding: '1.5rem' }}>
                            <div>
                                <div className="skeleton-title" style={{ height: '24px', width: '150px', marginBottom: '1rem' }}></div>
                                <div className="skeleton-text" style={{ height: '16px', width: '100px', marginBottom: '0.5rem' }}></div>
                                <div className="skeleton-text" style={{ height: '16px', width: '120px', marginBottom: '0.5rem' }}></div>
                            </div>
                            <div>
                                <div className="skeleton-text" style={{ height: '16px', width: '80px', marginBottom: '0.5rem' }}></div>
                                <div className="skeleton-text" style={{ height: '16px', width: '90px', marginBottom: '0.5rem' }}></div>
                            </div>
                            <div>
                                <div className="skeleton-badge" style={{ height: '24px', width: '80px', borderRadius: '20px' }}></div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            <style jsx>{`
                .skeleton-button {
                    background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
                    background-size: 200% 100%;
                    animation: loading 1.5s infinite;
                    border-radius: 50px;
                }
                
                .skeleton-filter, .skeleton-search, .skeleton-total {
                    background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
                    background-size: 200% 100%;
                    animation: loading 1.5s infinite;
                    border-radius: 8px;
                }
                
                .skeleton-title, .skeleton-text, .skeleton-badge {
                    background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
                    background-size: 200% 100%;
                    animation: loading 1.5s infinite;
                    border-radius: 4px;
                }
                
                .booking-card-skeleton {
                    background: white;
                    border: 1px solid #e0e0e0;
                    border-radius: 8px;
                    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
                }
                
                @keyframes loading {
                    0% { background-position: 200% 0; }
                    100% { background-position: -200% 0; }
                }
            `}</style>
        </div>
    );
}
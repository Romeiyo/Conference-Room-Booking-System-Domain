"use client";

export default function DashboardLoading() {
    return (
        <div className="container">
            <div className="header-section">
                <h1>Book a Conference Room</h1>
                <div className="skeleton-link" style={{ width: '120px', height: '40px' }}></div>
            </div>

            <div className="form-page-content">
                <div className="booking-form" style={{ padding: '2rem' }}>
                    <h3 style={{ marginBottom: '1.5rem', borderBottom: '2px solid #667eea', paddingBottom: '0.5rem' }}>
                        Book a New Room
                    </h3>

                    {[1, 2, 3, 4, 5, 6].map((i) => (
                        <div key={i} className="form-row" style={{ marginBottom: '1rem' }}>
                            <div style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
                                <div className="skeleton-label" style={{ width: '100px', height: '20px' }}></div>
                                <div className="skeleton-field" style={{ flex: 1, height: '40px' }}></div>
                            </div>
                        </div>
                    ))}

                    <div className="form-row" style={{ marginTop: '2rem' }}>
                        <div className="skeleton-button" style={{ width: '100%', height: '50px' }}></div>
                    </div>
                </div>
            </div>

            <style jsx>{`
                .skeleton-link, .skeleton-label, .skeleton-field, .skeleton-button {
                    background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
                    background-size: 200% 100%;
                    animation: loading 1.5s infinite;
                    border-radius: 4px;
                }
                
                .skeleton-field {
                    border-radius: 8px;
                }
                
                .skeleton-button {
                    border-radius: 50px;
                }
                
                @keyframes loading {
                    0% { background-position: 200% 0; }
                    100% { background-position: -200% 0; }
                }
            `}</style>
        </div>
    );
}
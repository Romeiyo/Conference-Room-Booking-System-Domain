import Link from 'next/link';
import '@/app/globals.css';

export default function HomePage() {
    return (
        <div style={{ textAlign: 'center', padding: '4rem 2rem' }}>
            <h1>Welcome to Conference Room Booking System</h1>
            <p style={{ fontSize: '1.2rem', margin: '2rem 0', color: '#666' }}>
                Book conference rooms easily and efficiently
            </p>
            <div style={{ display: 'flex', gap: '1rem', justifyContent: 'center' }}>
                <Link href="/login" className="btn">
                    Login
                </Link>
            </div>
        </div>
    );
}
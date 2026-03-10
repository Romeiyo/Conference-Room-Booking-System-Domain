import Link from 'next/link';
import './globals.css';

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body>
        <nav className="bg-gray-800 text-white p-4">
          <div className="container mx-auto flex justify-between">
            <Link href="/" className="font-bold">Conference Booking</Link>
            <div className="space-x-4">
              <Link href="/" className="hover:text-gray-300">Home</Link>
              <Link href="/dashboard" className="hover:text-gray-300">Dashboard</Link>
            </div>
          </div>
        </nav>
        <main className="container mx-auto p-4">{children}</main>
        <footer className="bg-gray-800 text-white text-center p-4 mt-8">
          © 2026 Conference Room Booking
        </footer>
      </body>
    </html>
  );
}
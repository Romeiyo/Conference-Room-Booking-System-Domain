import Link from 'next/link';

export default function HomePage() {
  return (
    <div className="text-center py-12">
      <h1 className="text-4xl font-bold mb-4">Conference Room Booking</h1>
      <p className="text-xl mb-8">Book rooms across Bloemfontein and Cape Town</p>
      <Link href="/dashboard" className="bg-blue-600 text-white px-6 py-3 rounded-lg text-lg">
        Go to Dashboard
      </Link>
    </div>
  );
}
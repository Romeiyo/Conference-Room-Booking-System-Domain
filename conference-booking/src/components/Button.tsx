'use client';

export default function Button({ label, onClick, type = 'button', disabled }: any) {
  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled}
      className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
    >
      {label}
    </button>
  );
}
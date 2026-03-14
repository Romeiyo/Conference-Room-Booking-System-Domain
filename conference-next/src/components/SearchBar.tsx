"use client";

import { useState, useEffect } from 'react';
import '@/app/globals.css';

interface SearchBarProps {
    onSearch: (term: string) => void;
    placeholder?: string;
    initialValue?: string;
}

export default function SearchBar({ onSearch, placeholder = "Search bookings...", initialValue = "" }: SearchBarProps) {
    const [searchInput, setSearchInput] = useState(initialValue);

    // Effect to trigger the actual search when debounced value changes
    useEffect(() => {
        onSearch(searchInput);
    }, [searchInput, onSearch]);

    return (
        <div className="search-container" style={{ marginBottom: '1rem' }}>
            <div style={{ position: 'relative' }}>
                <input
                    type="text"
                    value={searchInput}
                    onChange={(e) => setSearchInput(e.target.value)}
                    placeholder={placeholder}
                    className="search-input"
                    style={{
                        width: '100%',
                        padding: '0.75rem 1rem 0.75rem 2.5rem',
                        borderRadius: '8px',
                        border: '1px solid #e0e0e0',
                        fontSize: '1rem',
                        transition: 'all 0.3s ease',
                        outline: 'none'
                    }}
                />
                <span style={{
                    position: 'absolute',
                    left: '0.75rem',
                    top: '50%',
                    transform: 'translateY(-50%)',
                    color: '#999',
                    fontSize: '1.2rem'
                }}>🔍</span>
                {searchInput && (
                    <button
                        onClick={() => setSearchInput('')}
                        style={{
                            position: 'absolute',
                            right: '0.75rem',
                            top: '50%',
                            transform: 'translateY(-50%)',
                            background: 'none',
                            border: 'none',
                            color: '#999',
                            cursor: 'pointer',
                            fontSize: '1.2rem'
                        }}
                    >
                        ✕
                    </button>
                )}
            </div>
            <div style={{ fontSize: '0.85rem', color: '#666', marginTop: '0.25rem', textAlign: 'right' }}>
                {searchInput ? 'Typing...' : 'Search ready'}
            </div>
        </div>
    );
}
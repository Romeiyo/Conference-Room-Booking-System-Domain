"use client";

import '@/app/globals.css';

interface FilterProps {
    categoryFilter: string;
    onCategoryChange: (e: React.ChangeEvent<HTMLSelectElement>) => void;
    locations: string[];
}

export default function Filter({ categoryFilter, onCategoryChange, locations }: FilterProps) {
    return (
        <div className="filter-container">
            <label htmlFor="category-filter" className="filter-label">
                Filter by Location:
            </label>
            <select
                id="category-filter"
                value={categoryFilter}
                onChange={onCategoryChange}
                className="filter-select"
            >
                {locations.map((location, index) => (
                    <option key={index} value={location}>
                        {location}
                    </option>
                ))}
            </select>
            
            {categoryFilter !== 'All' && (
                <span className="active-filter">
                    Active filter: {categoryFilter}
                </span>
            )}
        </div>
    );
}
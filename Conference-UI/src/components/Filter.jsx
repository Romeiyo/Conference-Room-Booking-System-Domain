import '../App.css';

function Filter({ categoryFilter, onCategoryChange, locations }) {
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
        {locations.map(location => (
          <option key={location} value={location}>
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

export default Filter;
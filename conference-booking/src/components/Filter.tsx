export default function Filter({ filter, onChange, locations }: any) {
  return (
    <div className="mb-4">
      <label className="mr-2">Filter by Location:</label>
      <select
        value={filter}
        onChange={(e) => onChange(e.target.value)}
        className="border p-2 rounded"
      >
        {locations.map((loc: string) => (
          <option key={loc} value={loc}>{loc}</option>
        ))}
      </select>
      {filter !== 'All' && (
        <span className="ml-2 text-sm text-gray-600">Active: {filter}</span>
      )}
    </div>
  );
}
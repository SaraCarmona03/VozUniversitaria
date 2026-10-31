export default function SearchBar({ value, onChange }) {
  return (
    <label className="search-label">
      <span className="visually-hidden">Buscar publicaciones</span>
      <input
        className="post-search"
        type="search"
        placeholder="Buscar publicaciones"
        value={value}
        onChange={(event) => onChange(event.target.value)}
      />
    </label>
  );
}
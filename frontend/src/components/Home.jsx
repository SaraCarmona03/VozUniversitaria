import { useEffect, useMemo, useState } from "react";
import SearchBar from "./SearchBar.jsx";
import PostList from "./PostList.jsx";
import PostsSummary from "./PostsSummary.jsx";
import { usePostsStore } from "../store/postsStore.js";

const filters = [
  { label: "All", value: "all", className: "filter-all" },
  { label: "Complaints", value: "complaint", className: "filter-complaint" },
  { label: "Suggestions", value: "suggestion", className: "filter-suggestion" },
  { label: "Events", value: "event", className: "filter-event" },
  { label: "Notices", value: "notice", className: "filter-notice" },
];

export default function Home({ session, onLogout, isLoggingOut, error }) {
  const [activeFilter, setActiveFilter] = useState("all");
  const [search, setSearch] = useState("");

  const items = usePostsStore((s) => s.items);
  const status = usePostsStore((s) => s.status);
  const fetchPosts = usePostsStore((s) => s.fetchPosts);

  useEffect(() => {
    fetchPosts();
  }, [fetchPosts]);

  const cambiarStatus = () => {
    usePostsStore.setState((s) => ({ status: s.status === "success" ? "loading" : "success" }));
  };

  const visiblePosts = useMemo(() => {
    const query = search.trim().toLocaleLowerCase();

    return items.filter((post) => {
      const matchesCategory = activeFilter === "all" || post.categoryKey === activeFilter;
      const searchableText = [
        post.category,
        post.time,
        post.title,
        post.description,
        post.footerLabel,
        post.status,
      ]
        .filter(Boolean)
        .join(" ")
        .toLocaleLowerCase();

      return matchesCategory && (!query || searchableText.includes(query));
    });
  }, [items, activeFilter, search]);

  return (
    <div className="home-shell">
      <header className="site-header">
        <a className="brand" href="/Home/Index">VozUniversitaria</a>
        <nav className="main-nav" aria-label="Navegación principal">
          <a className="nav-active" href="/Home/Index">Home</a>
          <a href="/Home/Privacy">Privacy</a>
        </nav>
        <div className="account-menu">
          <span className="account-name">{session.name}</span>
          <button onClick={onLogout} disabled={isLoggingOut}>
            {isLoggingOut ? "Cerrando..." : "Cerrar sesión"}
          </button>
        </div>
      </header>

      <main className="home-content">
        {error && <p className="notice failure" role="alert">{error}</p>}
        <p className="welcome">Bienvenido, {session.name}</p>

        <section className="feed-section" aria-labelledby="feed-title">
          <div className="feed-heading">
            <div>
              <h1 id="feed-title">Your voice counts</h1>
              <p>Share complaints, suggestions, events, and notices with your community.</p>
            </div>
            <button className="new-post-button" type="button">+ Nueva publicación</button>
          </div>

          <PostsSummary />
          <button type="button" onClick={cambiarStatus}>Cambiar status (temporal)</button>
          <SearchBar value={search} onChange={setSearch} />

          <div className="post-filters" aria-label="Filtrar publicaciones por categoría">
            {filters.map((filter) => (
              <button
                key={filter.value}
                className={`filter-pill ${filter.className}${activeFilter === filter.value ? " selected" : ""}`}
                type="button"
                aria-pressed={activeFilter === filter.value}
                onClick={() => setActiveFilter(filter.value)}
              >
                {filter.label}
              </button>
            ))}
          </div>

          {status === "loading" && <p>Cargando publicaciones...</p>}
          {status === "error" && (
            <p className="notice failure" role="alert">No se pudieron cargar las publicaciones.</p>
          )}

          <PostList posts={visiblePosts} />
        </section>
      </main>
    </div>
  );
}
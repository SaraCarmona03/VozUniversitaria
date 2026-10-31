import { useEffect, useMemo, useState } from "react";
import PostCard from "./PostCard.jsx";
import CreatePublication from "./CreatePublication.jsx";
import ConfirmPublicationAction from "./ConfirmPublicationAction.jsx";
import mockPosts from "../data/mockPosts.js";
import {
  createPublication,
  deletePublication,
  getPublications,
  updatePublication,
} from "../api/publicationsApi.js";

const filters = [
  { label: "Todas", value: "all", className: "filter-all" },
  { label: "Quejas", value: "complaint", className: "filter-complaint" },
  { label: "Sugerencias", value: "suggestion", className: "filter-suggestion" },
  { label: "Eventos", value: "event", className: "filter-event" },
  { label: "Avisos", value: "notice", className: "filter-notice" },
];

function toFeedPost(publication) {
  return {
    ...publication,
    feedKey: `publication-${publication.id}`,
    categoryKey: publication.categoryKey,
    time: new Date(publication.createdAt).toLocaleString("es-MX", {
      dateStyle: "medium",
      timeStyle: "short",
    }),
    footerLabel: `Por ${publication.author}`,
  };
}

export default function Home({ session, onLogout, isLoggingOut, error, csrfToken }) {
  const [activeFilter, setActiveFilter] = useState("all");
  const [search, setSearch] = useState("");
  const [publications, setPublications] = useState([]);
  const [isLoadingPublications, setIsLoadingPublications] = useState(true);
  const [publicationsError, setPublicationsError] = useState("");
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const [createError, setCreateError] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [pendingAction, setPendingAction] = useState(null);
  const [actionError, setActionError] = useState("");
  const [isProcessingAction, setIsProcessingAction] = useState(false);
  const [editingPublication, setEditingPublication] = useState(null);

  useEffect(() => {
    let isMounted = true;
    getPublications()
      .then((data) => {
        if (isMounted) {
          setPublications(data.map(toFeedPost));
        }
      })
      .catch((requestError) => {
        if (isMounted) {
          setPublicationsError(requestError.message);
        }
      })
      .finally(() => {
        if (isMounted) {
          setIsLoadingPublications(false);
        }
      });

    return () => {
      isMounted = false;
    };
  }, []);

  const visiblePosts = useMemo(() => {
    const query = search.trim().toLocaleLowerCase();

    return [...publications, ...mockPosts].filter((post) => {
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
  }, [activeFilter, publications, search]);

  async function handleCreatePublication(publication) {
    setCreateError("");
    setIsCreating(true);
    try {
      const created = await createPublication(publication, csrfToken);
      setPublications((current) => [toFeedPost(created), ...current]);
      setIsCreateOpen(false);
      setSuccessMessage("Tu publicación se creó correctamente.");
    } catch (requestError) {
      setCreateError(requestError.message);
    } finally {
      setIsCreating(false);
    }
  }

  function requestPublicationAction(type, publication) {
    setActionError("");
    setPendingAction({ type, publication });
  }

  async function confirmPublicationAction() {
    if (!pendingAction) {
      return;
    }

    setActionError("");
    if (pendingAction.type === "edit") {
      setEditingPublication(pendingAction.publication);
      setPendingAction(null);
      return;
    }

    setIsProcessingAction(true);
    try {
      await deletePublication(pendingAction.publication.id, csrfToken);
      setPublications((current) =>
        current.filter((publication) => publication.id !== pendingAction.publication.id),
      );
      setPendingAction(null);
      setSuccessMessage("La publicación se eliminó correctamente.");
    } catch (requestError) {
      setActionError(requestError.message);
    } finally {
      setIsProcessingAction(false);
    }
  }

  async function handleUpdatePublication(publication) {
    setCreateError("");
    setIsCreating(true);
    try {
      const updated = await updatePublication(
        editingPublication.id,
        publication,
        csrfToken,
      );
      setPublications((current) =>
        current.map((item) =>
          item.id === updated.id ? toFeedPost(updated) : item,
        ),
      );
      setEditingPublication(null);
      setSuccessMessage("La publicación se actualizó correctamente.");
    } catch (requestError) {
      setCreateError(requestError.message);
    } finally {
      setIsCreating(false);
    }
  }

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
              <h1 id="feed-title">Voz Universitaria</h1>
              <p>Comparte quejas, sugerencias, eventos y avisos con tu comunidad.</p>
            </div>
            <button
              className="new-post-button"
              type="button"
              onClick={() => {
                setCreateError("");
                setSuccessMessage("");
                setIsCreateOpen(true);
              }}
            >
              + Nueva publicación
            </button>
          </div>

          {successMessage && <p className="notice success" role="status">{successMessage}</p>}
          {publicationsError && (
            <p className="notice failure" role="alert">
              No se pudieron cargar tus publicaciones: {publicationsError}
            </p>
          )}

          <label className="search-label">
            <span className="visually-hidden">Buscar publicaciones</span>
            <input
              className="post-search"
              type="search"
              placeholder="Buscar publicaciones"
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
          </label>

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

          {isLoadingPublications ? (
            <p className="empty-feed" role="status">Cargando publicaciones...</p>
          ) : visiblePosts.length > 0 ? (
            <div className="posts-grid">
              {visiblePosts.map((post) => (
                <PostCard
                  key={post.feedKey || `example-${post.id}`}
                  post={post}
                  canManage={String(post.authorId) === String(session.userId)}
                  onEdit={(publication) => requestPublicationAction("edit", publication)}
                  onDelete={(publication) => requestPublicationAction("delete", publication)}
                />
              ))}
            </div>
          ) : (
            <p className="empty-feed">No hay publicaciones que coincidan con tu búsqueda.</p>
          )}
        </section>
      </main>
      {isCreateOpen && (
        <CreatePublication
          onCancel={() => setIsCreateOpen(false)}
          onSubmit={handleCreatePublication}
          busy={isCreating}
          error={createError}
        />
      )}
      {pendingAction && (
        <ConfirmPublicationAction
          action={pendingAction.type}
          busy={isProcessingAction}
          error={actionError}
          onCancel={() => {
            setPendingAction(null);
            setActionError("");
          }}
          onConfirm={confirmPublicationAction}
        />
      )}
      {editingPublication && (
        <CreatePublication
          key={editingPublication.id}
          initialValues={{
            categoryKey: editingPublication.categoryKey,
            title: editingPublication.title,
            description: editingPublication.description,
          }}
          isEditing
          onCancel={() => setEditingPublication(null)}
          onSubmit={handleUpdatePublication}
          busy={isCreating}
          error={createError}
        />
      )}
    </div>
  );
}

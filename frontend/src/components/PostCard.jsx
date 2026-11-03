export default function PostCard({ post, canManage, onEdit, onDelete }) {
  return (
    <article className="post-card">
      <div className="post-card-top">
        <span className={`category-badge category-${post.categoryKey}`}>{post.category}</span>
        <time className="post-time">{post.time}</time>
      </div>
      <h2 className="post-title">{post.title}</h2>
      <p className="post-description">{post.description}</p>
      <div className="post-footer">
        <div className="post-footer-primary">
          {post.status && <span className="status-badge">{post.status}</span>}
          <span>{post.footerLabel}</span>
        </div>
        {post.commentsLabel && <span className="post-comments">{post.commentsLabel}</span>}
      </div>
      {canManage && (
        <div className="post-management" aria-label="Administrar publicación">
          <button
            className="post-action edit-action"
            type="button"
            onClick={() => onEdit(post)}
            aria-label={`Editar ${post.title}`}
            title="Editar publicación"
          >
            <svg aria-hidden="true" viewBox="0 0 24 24">
              <path d="M12 20h9" />
              <path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L8 18l-4 1 1-4Z" />
            </svg>
          </button>
          <button
            className="post-action delete-action"
            type="button"
            onClick={() => onDelete(post)}
            aria-label={`Eliminar ${post.title}`}
            title="Eliminar publicación"
          >
            <svg aria-hidden="true" viewBox="0 0 24 24">
              <path d="M3 6h18" />
              <path d="M8 6V4h8v2" />
              <path d="m19 6-1 14H6L5 6" />
              <path d="M10 11v5M14 11v5" />
            </svg>
          </button>
        </div>
      )}
    </article>
  );
}

export default function PostCard({ post }) {
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
    </article>
  );
}

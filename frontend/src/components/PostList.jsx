import PostCard from "./PostCard.jsx";

export default function PostList({ posts }) {
  if (posts.length === 0) {
    return <p className="empty-feed">No hay publicaciones que coincidan con tu búsqueda.</p>;
  }

  return (
    <div className="posts-grid">
      {posts.map((post) => <PostCard key={post.id} post={post} />)}
    </div>
  );
}
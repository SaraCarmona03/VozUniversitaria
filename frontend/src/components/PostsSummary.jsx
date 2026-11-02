import { memo } from "react";
import { usePostsStore } from "../store/postsStore.js";

function PostsSummary() {
  const items = usePostsStore((s) => s.items); // selector estrecho
  console.count("PostsSummary render");
  const complaints = items.filter((p) => p.categoryKey === "complaint").length;

  return <p>Total: {items.length} · Quejas: {complaints}</p>;
}

export default memo(PostsSummary);
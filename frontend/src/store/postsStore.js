import { create } from "zustand";
import { getPosts } from "../api/posts.js";

export const usePostsStore = create((set, get) => ({
  items: [],
  status: "idle", // idle | loading | success | error
  error: "",

  fetchPosts: async () => {
    const { status } = get();
    if (status === "loading" || status === "success") return;

    set({ status: "loading", error: "" });
    try {
      const items = await getPosts();
      set({ items, status: "success" });
    } catch (err) {
      set({ status: "error", error: err.message });
    }
  },
}));
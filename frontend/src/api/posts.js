import { request } from "./client.js";

export function getPosts() {
  return request("/api/posts");
}
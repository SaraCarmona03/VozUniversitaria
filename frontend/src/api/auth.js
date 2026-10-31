  import { request } from "./client.js";

  export async function getCsrfToken() {
    const result = await request("/api/security/csrf");
    return result.requestToken;
  }
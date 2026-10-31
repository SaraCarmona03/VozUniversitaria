const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || "").replace(/\/$/, "");

export class ApiError extends Error {
  constructor(status, message, details = null) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.details = details;
  }
}

export async function request(path, options = {}) {
  let response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      credentials: "include",
      ...options,
      headers: {
        ...(options.body ? { "Content-Type": "application/json" } : {}),
        ...options.headers,
      },
    });
  } catch {
    // Network failure: the server did not answer (status 0).
    throw new ApiError(0, "No se pudo conectar con el servidor. Intenta de nuevo en un momento.");
  }

  if (response.status === 204) {
    return null;
  }

  const contentType = response.headers.get("content-type") || "";
  const body = contentType.includes("application/json")
    ? await response.json()
    : null;

  if (!response.ok) {
    // HTTP error: the server answered, but with a failure status.
    const fallback = response.status === 404
      ? "No se encontró lo que buscabas (404)."
      : `No se pudo completar la solicitud (error ${response.status}).`;
    throw new ApiError(response.status, body?.error || body?.title || fallback, body);
  }

  return body;
}
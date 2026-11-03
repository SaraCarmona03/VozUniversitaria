async function request(path, options = {}) {
  const response = await fetch(`/api/publications${path}`, {
    credentials: "include",
    ...options,
    headers: {
      ...(options.body ? { "Content-Type": "application/json" } : {}),
      ...options.headers,
    },
  });

  const contentType = response.headers.get("content-type") || "";
  const body = contentType.includes("application/json")
    ? await response.json()
    : null;

  if (!response.ok) {
    throw new Error(body?.error || body?.title || "No se pudo completar la solicitud.");
  }

  return body;
}

export function getPublications() {
  return request("");
}

export function createPublication(publication, csrfToken) {
  return request("", {
    method: "POST",
    headers: { "X-CSRF-TOKEN": csrfToken },
    body: JSON.stringify(publication),
  });
}

export function updatePublication(id, publication, csrfToken) {
  return request(`/${id}`, {
    method: "PUT",
    headers: { "X-CSRF-TOKEN": csrfToken },
    body: JSON.stringify(publication),
  });
}

export function deletePublication(id, csrfToken) {
  return request(`/${id}`, {
    method: "DELETE",
    headers: { "X-CSRF-TOKEN": csrfToken },
  });
}

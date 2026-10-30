export const getNoticias = async () => {
  const response = await fetch(
    "https://jsonplaceholder.typicode.com/posts?_limit=5",
  );
  if (!response.ok) {
    throw new Error(`Error al consultar noticias (${response.status}).`);
  }

  const data = await response.json();
  if (!Array.isArray(data)) {
    throw new Error("La API devolvió un formato de noticias inválido.");
  }

  return data.slice(0, 5);
};
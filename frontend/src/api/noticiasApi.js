// src/api/noticiasApi.js
export const getNoticias = async () => {
  const response = await fetch('https://jsonplaceholder.typicode.com/posts?_limit=5');
  if (!response.ok) {
    throw new Error('Error al consultar la API');
  }
  return await response.json();
};
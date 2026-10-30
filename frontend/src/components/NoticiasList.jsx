import React, { useEffect } from 'react';
import { useNoticiasStore } from '../store/useNoticiasStore';

export const NoticiasList = () => {
  // Selector específico: solo leemos lo que necesita este componente
  const items = useNoticiasStore((state) => state.items);
  const isLoading = useNoticiasStore((state) => state.isLoading);
  const error = useNoticiasStore((state) => state.error);
  const fetchItems = useNoticiasStore((state) => state.fetchItems);

  useEffect(() => {
    fetchItems(); // Llama la acción del store (el guard evita peticiones dobles)
  }, [fetchItems]);

  if (isLoading) return <p>Cargando noticias...</p>;
  if (error) {
    return (
      <div role="alert">
        <p>No se pudieron cargar las noticias: {error}</p>
        <button onClick={fetchItems}>Reintentar</button>
      </div>
    );
  }

  return (
    <div>
      <h2>Lista de Noticias</h2>
      <ul>
        {items.map((item) => (
          <li key={item.id}>{item.title}</li>
        ))}
      </ul>
    </div>
  );
};
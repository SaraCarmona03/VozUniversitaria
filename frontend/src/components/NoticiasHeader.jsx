import React from 'react';
import { useNoticiasStore } from '../store/useNoticiasStore';

export const NoticiasHeader = () => {
  // Selector específico
  const totalNoticias = useNoticiasStore((state) => state.items.length);

  return (
    <header>
      <h3>Resumen Universitaro</h3>
      <p>Total de noticias cargadas: {totalNoticias}</p>
    </header>
  );
};
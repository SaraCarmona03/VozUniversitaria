import { create } from 'zustand';
import { getNoticias } from '../api/noticiasApi'; // Ajusta la ruta real de tu servicio API

export const useNoticiasStore = create((set, get) => ({
  items: [],
  isLoading: false,
  hasFetched: false, // Mecanismo de control (guard) para evitar múltiples peticiones

  // Acción que realiza el fetch
  fetchItems: async () => {
    if (get().hasFetched) return; // Guard para evitar la segunda petición

    set({ isLoading: true });
    try {
      const data = await getNoticias();
      set({ items: data, hasFetched: true, isLoading: false });
    } catch (error) {
      console.error("Error al obtener datos:", error);
      set({ isLoading: false });
    }
  },

  // Función set para actualizar/mutar de forma inmutable
  setItems: (items) => set({ items }),

  // Método temporal para la prueba de fallo (mutación)
  addItemTemp: (item) => set((state) => ({ items: [...state.items, item] })),
}));
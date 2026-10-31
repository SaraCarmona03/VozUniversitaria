import { create } from 'zustand';
import { getNoticias } from '../api/noticiasApi'; // Ajusta la ruta real de tu servicio API

export const useNoticiasStore = create((set, get) => ({
  items: [],
  isLoading: false,
  hasFetched: false, // Mecanismo de control (guard) para evitar múltiples peticiones
  error: null,

  fetchItems: async () => {
    if (get().hasFetched || get().isLoading) return;

    set({ isLoading: true, error: null });
    try {
      const data = await getNoticias();
      set({ items: data, hasFetched: true, isLoading: false, error: null });
    } catch (error) {
      console.error("Error al obtener datos:", error);
      set({
        isLoading: false,
        error: error instanceof Error ? error.message : "No se pudieron cargar las noticias.",
      });
    }
  },

  setItems: (items) => set({ items }),

  addItemTemp: (item) => set((state) => ({ items: [...state.items, item] })),
}));
import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  base: "/app/",
  plugins: [react()],
  server: {
    proxy: {
      "/api": {
        target: "http://localhost:5032",
        changeOrigin: true,
      },
    },
  },
  build: {
    outDir: fileURLToPath(new URL("../VozUniversitaria/wwwroot/app", import.meta.url)),
    emptyOutDir: true,
  },
});

import react from "@vitejs/plugin-react";
import path from "path";
import { defineConfig } from "vite";

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    port: 3000,
    host: true,
    // Proxy /api to the backend so the dev server acts like nginx in production.
    // VITE_PROXY_TARGET is set by docker-compose.override.yml when running in Docker
    // (where the backend is reachable at http://api:8080, not localhost).
    proxy: {
      "/api": {
        target: process.env.VITE_PROXY_TARGET ?? "http://localhost:5000",
        changeOrigin: true,
      },
    },
  },
  preview: {
    port: 3000,
    host: true,
  },
});

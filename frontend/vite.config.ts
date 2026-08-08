import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
// The built SPA is emitted into the ASP.NET Core app's wwwroot so the MVC
// backend can serve it as a single application. A manifest is generated so a
// Razor view can reference the hashed asset filenames.
export default defineConfig({
  plugins: [react()],
  build: {
    outDir: "../backend/wwwroot",
    emptyOutDir: true,
    manifest: true,
  },
  // During `vite dev`, proxy API calls to the ASP.NET Core backend so the
  // frontend can stay same-origin (no VITE_API_BASE_URL needed).
  server: {
    proxy: {
      "/api": "http://localhost:5133",
      "/rss.xml": "http://localhost:5133",
    },
  },
})

import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
  ],
  server: {
    port: 3000,
    host: "riff-dev.local",
    hmr: {
        host: 'riff-dev.local',
        clientPort: 80
    },
    allowedHosts: ['riff-dev.local']
  }
})
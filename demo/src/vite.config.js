import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import fable from "vite-plugin-fable"

export default defineConfig({
    plugins: [react(), fable()],
    server: {
        port: 8080,
    }
})

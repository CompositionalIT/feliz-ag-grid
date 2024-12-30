import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import fable from "vite-plugin-fable"

export default defineConfig({
    plugins: [react(), fable()],
    server: {
        port: 8080,
    },
    resolve :{
        alias: {
            //because we are using project references instead of package references, we need this to allow loading the ag-grid-react module from the wrapper itself
            'ag-grid-react' :"../demo/src/node_modules/ag-grid-react"
        }
    }
})

import { defineConfig } from "vite";

export default defineConfig({
    build: {
        lib: {
            entry: {
                keyvaluelist: "src/key-value-list-editor.element.ts",
                keyvaluetags: "src/key-value-tags-editor.element.ts"
            },
            formats: ["es"],
        },
        outDir: "../../wwwroot/App_Plugins/Client",
        emptyOutDir: true,
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/],
        },
    },
    base: "/App_Plugins/Client/",
});
/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./Pages/**/*.razor",
        "./Components/**/*.razor",
        "./Pages/*.razor",
        "./wwwroot/**/*.html",
        "./Layout/*.razor",
        "./Layouts/**/*.razor"
    ],
    theme: {
        extend: {
            colors: {
                // Mapper les couleurs de Tailwind à vos variables CSS
                primary: 'var(--color-primary)',
                secondary: 'var(--color-secondary)',
                text: 'var(--color-text)',
                hover: 'var(--color-hover)',
                active: 'var(--color-active)',
                focus: 'var(--color-focus)',
                white: 'var(--color-white)',
                fond_transparent: 'var(--color-soft)'
            },
        },
    },
    plugins: [],
}
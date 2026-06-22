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
                fond_transparent: 'var(--color-soft)',

                // --- NOUVEAUX : Textes ---
                title: 'var(--color-title)',
                subtitle: 'var(--color-subtitle)',
                body: 'var(--color-body)',
                label: 'var(--color-label)',
                muted: 'var(--color-muted)',
                placeholder: 'var(--color-placeholder)',

                // --- NOUVEAUX : Accent ---
                accent: 'var(--color-accent)',
                'accent-hover': 'var(--color-accent-hover)',
                'accent-active': 'var(--color-accent-active)',

                // --- NOUVEAUX : Surfaces ---
                surface: 'var(--color-surface)',
                'surface-alt': 'var(--color-surface-alt)',
                'section-alt': 'var(--color-section-alt)',
                'tag-bg': 'var(--color-tag-bg)',

                // --- NOUVEAUX : Statuts ---
                success: 'var(--color-success)',
                'success-bg': 'var(--color-success-bg)',
                'success-light-bg': 'var(--color-success-light-bg)',
                info: 'var(--color-info)',
                'info-bg': 'var(--color-info-bg)',
                'info-light-bg': 'var(--color-info-light-bg)',
                warning: 'var(--color-warning)',
                danger: 'var(--color-danger)',
                'danger-hover-bg': 'var(--color-danger-hover-bg)',

                // --- NOUVEAUX : Bordures ---
                'border-soft': 'var(--color-border-soft)',
                border: 'var(--color-border)',
                'border-input': 'var(--color-border-input)',
            },
        },
    },
    plugins: [],
}
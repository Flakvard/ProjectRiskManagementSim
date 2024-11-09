/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./**/*.{razor,html,cshtml}"],
  theme: {
    extend: {
      keyframes: {
            shimmer: {
              '100%': {
                transform: 'translateX(100%)',
              },
            },
          },
    },
  },
  plugins: [],
}


/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}"
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['"DM Sans"', 'sans-serif'],
        dm: ['"DM Sans"', 'sans-serif'],
        montserrat: ['"Montserrat"', 'sans-serif'],
        raleway: ['"Raleway"', 'sans-serif'],
        lora: ['"Lora"', 'serif', 'italic'],
      },
      colors: {
        'clr-DeepPlum': '#5D3A9B',
        'clr-SoftCoral': '#FF6B6B',
        'clr-LightIvory': '#F9F6F1',
        'clr-CharcoalGray': '#333333',
        'clr-WarmTaupe': '#C1B8A3',
        'clr-BurntOrange': '#D9822B',
        'clr-babyPink': '#FFD9DA',
        
        'primary-cta': '#1C2155',
        'soft-pink':'#FFD9DA',
        'primary-section': '#065A82',
        'muted-purple':'#9882AC',
        'primary-header':'#BB8037',
        'light-gray':'#D4D4D4'
      },
    },
  },
  plugins: [],
}
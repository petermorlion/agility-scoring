/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./app/**/*.{js,jsx,ts,tsx}"],
  presets: [require("nativewind/preset")],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#6e8b3d',
          50: '#dbe6c7',
          100: '#cfdfb5',
          200: '#b8cf92',
          300: '#a1c06e',
          400: '#8aae4d',
          500: '#6e8b3d',
          600: '#52682d',
          700: '#36441e',
          800: '#1a210e',
          900: '#000000',
          950: '#000000'
        },
        secondary: {
          DEFAULT: '#8b4513',
          50: '#f0ba93',
          100: '#edac7d',
          200: '#e78f50',
          300: '#e17223',
          400: '#b85b19',
          500: '#8b4513',
          600: '#5e2f0d',
          700: '#311807',
          800: '#040201',
          900: '#000000',
          950: '#000000'
        },
        accent: {
          DEFAULT: '#4682b4',
          50: '#e9f0f6',
          100: '#d7e4ef',
          200: '#b2cce1',
          300: '#8db3d3',
          400: '#699bc4',
          500: '#4682b4',
          600: '#38678f',
          700: '#294d6b',
          800: '#1b3246',
          900: '#0d1821',
          950: '#060b0f'
        }
      }
    },
  },
  plugins: [],
}


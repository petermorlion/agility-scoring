# Agility Scoring Web Application

This is the web version of the Agility Scoring application, converted from the original Expo mobile app.

## 🚀 Getting Started

### Prerequisites

- Node.js v18+
- npm or yarn
- The API server running (see `/apps/api`)

### Installation

```bash
cd apps/web
npm install
```

### Development

```bash
npm run dev
```

This will start the Vite development server on `http://localhost:3001`

### Build

```bash
npm run build
```

### Preview Production Build

```bash
npm run preview
```

## 📁 Project Structure

```
apps/web/
├── src/
│   ├── components/      # Reusable UI components
│   ├── pages/           # Page components
│   ├── utils/           # Utility functions and hooks
│   ├── App.tsx          # Main application component
│   ├── main.tsx         # Entry point
│   └── index.css        # Global styles
├── public/             # Static assets
├── vite.config.ts      # Vite configuration
├── tailwind.config.js  # Tailwind CSS configuration
└── package.json        # Dependencies and scripts
```

## 🔧 Configuration

### API Connection

The web app connects to the API server via tRPC. The configuration is in:
- `src/utils/trpc.ts` - tRPC client setup
- `vite.config.ts` - Proxy configuration for development

### Google Authentication

To enable Google login:

1. Set up Google OAuth credentials in Google Cloud Console
2. Update the client ID in `src/pages/LoginPage.tsx`
3. Configure the redirect URI to match your domain

## 🎨 Technologies

- **Framework**: React 18
- **Routing**: React Router v6
- **State Management**: React Query (TanStack Query)
- **API**: tRPC
- **Styling**: Tailwind CSS
- **Build Tool**: Vite
- **Authentication**: Better-Auth with Google OAuth2

## 📱 Mobile to Web Conversion

### Key Changes Made:

1. **Replaced Expo-specific dependencies** with web equivalents:
   - `expo-router` → `react-router-dom`
   - `react-native` → `react-dom`
   - `react-native-vector-icons` → `react-icons`

2. **Updated navigation system** from file-based to route-based

3. **Converted mobile UI components** to web-friendly equivalents:
   - `TouchableOpacity` → `button` with proper styling
   - `View` → `div` with Tailwind classes
   - `Text` → Regular text with proper semantic HTML

4. **Updated build configuration** from Metro to Vite

5. **Maintained all functionality** including:
   - Tournament management
   - Google authentication
   - tRPC API integration
   - Multi-language support (structure preserved)

## 🔄 API Integration

The web app connects to the same tRPC API as the mobile app. Ensure the API server is running on `http://localhost:3000` for development.

## 🌐 Deployment

The app is configured for easy deployment to platforms like:
- Vercel
- Netlify
- Cloudflare Pages
- Any static hosting service

Build output is generated in the `dist/` directory.

## 📝 Notes

- The Google OAuth flow is implemented but requires proper configuration for production use
- All mobile-specific features have been adapted for web use
- The UI has been optimized for both desktop and mobile web browsers
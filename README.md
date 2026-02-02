# Agility Scoring Monorepo

A Turborepo monorepo containing a mobile app and API server.

## What's inside?

This monorepo includes the following apps:

### Apps

- `mobile`: An [Expo](https://expo.dev) React Native mobile app
- `api`: An [Express](https://expressjs.com) server with [tRPC](https://trpc.io) endpoints

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- For mobile development: [Expo Go app](https://expo.dev/go) on your phone or an emulator
- For WSL development and Android: follow [these steps](https://www.tutorialpedia.org/blog/how-to-run-android-emulator-on-windows-while-using-wsl2-with-zsh/)

### Installation

Install dependencies:

```bash
npm install
```

### Development

Run all apps in development mode:

```bash
npm run dev
```

This will start:
- The API server on `http://localhost:3000`
- The Expo development server

### Building

Build all apps:

```bash
npm run build
```

## Project Structure

```
.
├── apps
│   ├── api/          # Express + tRPC API server
│   └── mobile/       # Expo React Native app
├── packages/         # Shared packages (optional)
├── turbo.json        # Turborepo configuration
└── package.json      # Root package.json
```

## Technology Stack

- **Monorepo**: Turborepo
- **Mobile**: Expo, React Native, TypeScript
- **API**: Express, tRPC, TypeScript
- **Type Safety**: End-to-end type safety with tRPC

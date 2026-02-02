# Mobile App

Expo React Native mobile app for Agility Scoring.

## Development

Start the Expo development server:

```bash
npm run dev
```

Then:
- Press `i` for iOS simulator
- Press `a` for Android emulator
- Scan QR code with Expo Go app on your phone

## API Connection

The app connects to the API server at `http://localhost:3000/trpc`.

**Important**: Update the API URL in `app/_layout.tsx` based on your setup:
- iOS Simulator: `http://localhost:3000`
- Android Emulator: `http://10.0.2.2:3000`
- Physical device: `http://<your-computer-ip>:3000`

To find your computer's IP:
- Mac: `ifconfig | grep "inet " | grep -v 127.0.0.1`
- Linux: `ip addr show | grep "inet " | grep -v 127.0.0.1`
- Windows: `ipconfig` and look for IPv4 Address

## Features

- tRPC client integration
- React Query for data fetching
- End-to-end type safety with the API
- Expo Router for navigation

## Tech Stack

- Expo
- React Native
- tRPC Client
- React Query
- TypeScript
- [Material Design Icons](https://pictogrammers.com/library/mdi/)

# Agility Scoring .NET MAUI App

This is the .NET MAUI version of the Agility Scoring mobile application, converted from the original Expo React Native app.

## Features

- **Tournament Management**: Create, view, and manage agility tournaments
- **Authentication**: Google OAuth login using MSAL
- **Cross-Platform**: Runs on Android, iOS, and potentially other platforms
- **Modern UI**: Clean, responsive interface with MAUI Community Toolkit
- **API Integration**: Connects to the existing tRPC backend

## Project Structure

```
AgilityScoring.Maui/
├── Services/              # Core services (Auth, API, Config, Navigation)
├── ViewModels/            # MVVM ViewModels
├── Views/                 # XAML pages
├── Converters/            # Value converters
├── Resources/             # Styles, images, fonts
└── App.xaml.cs            # Application entry point
```

## Setup

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 with MAUI workload or VS Code with MAUI extension
- Android/iOS development environment

### Configuration

1. **Google OAuth Setup**:
   - Replace the placeholder client IDs in `ConfigService.cs` with your actual Google OAuth credentials
   - Configure the redirect URI in your Google Cloud Console: `agility-scoring-maui://oauth2redirect`

2. **API Configuration**:
   - Update the `ApiUrl` in `ConfigService.cs` to point to your backend API
   - Ensure your backend supports CORS for the MAUI app

### Running the App

```bash
# Navigate to the MAUI project directory
cd apps/maui

# Run on Android
 dotnet build -t:Run -f net8.0-android

# Run on iOS (requires Mac)
 dotnet build -t:Run -f net8.0-ios
```

## Key Components

### Authentication
- Uses Microsoft.Identity.Client (MSAL) for Google OAuth
- Secure storage for user sessions
- Automatic token refresh

### API Service
- HTTP client for tRPC endpoint communication
- Strongly-typed DTOs for tournament data
- Error handling and retry logic

### Navigation
- MAUI Shell for route-based navigation
- Redirect handling for authentication flows
- Deep linking support

## Conversion Notes

This MAUI app was converted from the original Expo React Native app. Key differences:

1. **Navigation**: Expo Router → MAUI Shell
2. **State Management**: React Query → MVVM with ViewModels
3. **Authentication**: Better Auth → MSAL
4. **Styling**: Tailwind CSS → XAML Styles
5. **API Client**: tRPC client → HttpClient

## Future Enhancements

- [ ] Implement proper internationalization with RESX files
- [ ] Add tournament detail view
- [ ] Implement contestant management
- [ ] Add performance tracking features
- [ ] Improve error handling and user feedback
- [ ] Add unit tests for ViewModels and Services

## Troubleshooting

### Android Build Issues
- Ensure you have the latest Android SDK installed
- Check that Java JDK 17 is installed and configured
- Verify Android emulator is properly set up

### iOS Build Issues
- Requires Xcode on macOS
- Ensure proper provisioning profiles are configured
- Check entitlements for Google OAuth

### Authentication Problems
- Verify Google OAuth credentials are correct
- Check that the redirect URI matches exactly
- Ensure the app is properly signed for production builds

## License

This project is licensed under the same license as the original Agility Scoring project.
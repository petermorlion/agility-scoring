# Kaylee — History

## Project Context
- **Project:** Agility Scoring — dog agility competition scoring app
- **Owner:** Peter
- **Stack:** .NET MAUI C# in `apps/maui`, targeting net10.0-android
- **Description:** Mobile app for creating dog agility tournaments and counting obstacle faults/refusals per run
- **Team:** Mal (Lead), Zoe (Backend Dev), Kaylee (MAUI Dev), Simon (Tester), Scribe, Ralph

## Learnings

### 2026-06-09 — Google OAuth: WebAuthenticator custom scheme → reverse client ID scheme
- Google deprecated arbitrary custom URI schemes (e.g. `agility-scoring-maui://`) for OAuth redirects
- Migrated to the reverse client ID URI scheme: `com.googleusercontent.apps.{client-id}:/oauth2redirect`
- This format is Google-approved for native Android apps; WebAuthenticator + PKCE flow unchanged
- Only two files changed: `WebAuthCallbackActivity.cs` (DataScheme + DataPath) and `ConfigService.cs` (RedirectUri)
- Rejected the native Google Sign-In NuGet approach (`Xamarin.Google.Android.Play.Services.Auth`) due to compatibility risk with `net10.0-android` and MAUI 8.x Activity lifecycle wiring
- **Google Cloud Console note:** If the credential is Android-type, no Cloud Console changes needed. If Web-type, add the reverse client ID redirect URI to Authorised Redirect URIs.

### 2026-02-23 — AuthConfig DI fix
- `AuthService` requires `AuthConfig` via constructor injection
- `AuthConfig` was not registered in `MauiProgram.cs` — caused runtime DI resolution error
- Fix: register `new AuthConfig { ... }` as singleton before `AuthService` registration
- Placeholder credential strings must be replaced by the developer with real OAuth values

### 2026-02-23 — MSAL → WebAuthenticator migration (Google OAuth)
- MSAL (`Microsoft.Identity.Client`) is for Azure AD/Entra — wrong for Google login
- Replaced with MAUI's built-in `WebAuthenticator` using PKCE-based Google OAuth flow
- `WebAuthenticatorCallbackActivity` requires an `[IntentFilter]` attribute in a new platform-specific file (`Platforms/Android/WebAuthCallbackActivity.cs`)
- Android native OAuth clients (Google) do NOT require `client_secret` for token exchange — only `client_id`, `code`, `code_verifier`, `redirect_uri`, `grant_type`
- PKCE implementation: generate random 32-byte code verifier → SHA256 hash → base64url encode = code challenge
- ID token is a JWT: split on `.`, base64url decode the middle segment, deserialize JSON to get `sub` (user ID), `name`, `email`
- Store refresh token in `SecureStorage` to enable silent token refresh in `CheckAuthAsync()`
- Token expiry should have a 5-minute buffer to avoid edge-case failures

### 2026-02-23 — Android deployment attempt
- Device `R3CX500VME` initially appeared as "offline" in `adb devices`
- After `adb kill-server` + `adb start-server`, device disappeared entirely
- Build compiled successfully (114 warnings, 0 errors) in 47.7s
- Deployment failed with **error XA0010: No available device** — device must be connected and authorized via ADB
- **To deploy:** 
  1. Ensure USB debugging is enabled on the Android device
  2. Accept the ADB authorization prompt on the phone
  3. Verify device shows as online in `adb devices` (not "offline" or "unauthorized")
  4. Then run: `dotnet build -t:Run -f net10.0-android` from `apps/maui/`

### 2026-02-23 — Android deployment SUCCESS
- After user accepted USB debugging prompt, device `R3CX500VMEL` showed as "device" (authorized) in `adb devices`
- Build and deployment succeeded in 53.4 seconds using `dotnet build -t:Run -f net10.0-android`
- App deployed to physical Android device successfully via ADB
- Key: Device must show status "device" (not "unauthorized" or "offline") in `adb devices` output before deployment will work

### 2026-06-09 — TournamentDetailPage: Shell navigation + IQueryAttributable pattern
- Created `TournamentDetailPage` with contestant navigation (←/→ arrows)
- **Shell navigation to pushed pages:** Use `Routing.RegisterRoute("route-name", typeof(MyPage))` in `AppShell.xaml.cs` constructor for pages that aren't tabs
- Pass query params via `Shell.Current.GoToAsync($"route?param={value}")` — URL-encode values with `Uri.EscapeDataString()`
- **IQueryAttributable:** ViewModels implement `IQueryAttributable.ApplyQueryAttributes(query)` to receive query params; decode with `Uri.UnescapeDataString()`
- **RelayCommand CanExecute:** Use `[RelayCommand(CanExecute = nameof(PropertyName))]` to auto-bind button IsEnabled — must call `Command.NotifyCanExecuteChanged()` when property changes
- **TapGestureRecognizer binding:** Use `Command="{Binding Source={RelativeSource AncestorType={x:Type vm:MyViewModel}}, Path=CommandName}"` to bind to parent ViewModel command from DataTemplate
- **NavigationPage.HasNavigationBar="False"** hides the default Shell navigation bar for custom title bars
- Contestant counter starts at 1 (no upper limit) — left arrow disabled when on contestant 1
- Build: 0 errors (pre-existing warning in AddTournamentViewModel unrelated)

### 2026-03-13 — Disqualified Toggle Button (Issue #18)
- Added disqualified functionality to TournamentDetailPage for marking contestants as disqualified
- **LocalStorageService extension:** Added `ContestantDisqualified` dictionary to `TournamentDto` to persist DQ status per contestant
- **ViewModel properties:** Added `IsDisqualified` (bool), `DisqualifiedButtonText` (computed string) to `TournamentDetailViewModel`
- **Toggle command:** `ToggleDisqualifiedCommand` flips local state, persists to storage, rolls back on error with alert
- **UI:** Added full-width button below contestant nav bar with dynamic text ("Mark as Disqualified" / "Disqualified ✓")
- **Visual feedback:** Created `BoolToDisqualifiedColorConverter` — gray when not DQ'd (#6c757d), red when DQ'd (#dc3545)
- **State sync:** DQ status loads when contestant number changes; persists in `_contestantDisqualified` dictionary
- **Backward compatibility:** Gracefully handles missing `ContestantDisqualified` in existing tournament JSON (defaults to false via `GetValueOrDefault`)
- Build: 0 errors, 41 warnings (pre-existing nullability warnings)
- **Architecture note:** App uses LocalStorage only; no API integration yet. Future API work will require mapping contestant numbers to result IDs.
- **Cross-team context:** Zoe implemented backend API; Simon wrote tests. API uses result `id` (UUID) but MAUI tracks by `contestantNumber` — integration task will need mapping layer.




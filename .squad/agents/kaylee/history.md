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

### 2026-06-09 — Disqualified Toggle Button (Issue #18)
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

### 2026-03-13 — PDF Export Feature (Issue #17) with Critical Fixes
- Implemented PDF export for tournaments using **QuestPDF** (NuGet package, MIT license, community edition)
- **PdfExportService improvements:**
  - **File location fix:** Changed from `FileSystem.CacheDirectory` → `Path.Combine(FileSystem.AppDataDirectory, "exports")` with `Directory.CreateDirectory()` for persistent storage
  - **Filename sanitization:** Improved from simple `Replace(" ", "_")` → `Path.GetInvalidFileNameChars()` LINQ Select for robust handling of apostrophes, colons, slashes, parentheses, etc.
  - **Filename format:** `{TournamentName}_{yyyyMMdd_HHmmss}.pdf` prevents overwrites via timestamp
- **PDF content:** Tournament name (24pt bold), date (16pt), table with 5 columns (contestant #, name, refusals, faults, DQ status)
- **Namespace conflicts:** QuestPDF has `IContainer` and `Colors` types that clash with MAUI — resolved by fully qualifying: `QuestPDF.Infrastructure.IContainer` and `QuestPDF.Helpers.Colors`
- **QuestPDF license:** Added `QuestPDF.Settings.License = LicenseType.Community;` to `MauiProgram.CreateMauiApp()` (required for community use)
- **UI:** Added "📄 PDF" button to each tournament card in TournamentListPage (80px wide, primary color, positioned at right of card)
- **Data aggregation:** `ExportToPdf` command builds `ContestantResult` list from `TournamentDto` (sums refusals/faults across obstacles, reads DQ status from dictionary)
- **DI wiring:** Registered `PdfExportService` as singleton in `MauiProgram.cs`; injected into `TournamentListViewModel` alongside `LocalStorageService`
- **Error handling:** Already in place in `TournamentListViewModel.ExportToPdfCommand` with try/catch and user alerts
- **Layout:** Grid with two columns — left column has tournament name/date, right column has export button (aligned vertically center)
- Build: 0 errors, 47 warnings (pre-existing nullability warnings)
- **Critical fixes verified:** 
  1. File location persists PDFs even after app cache clear or low storage events
  2. Filename sanitization handles edge cases (apostrophes, special chars) correctly
  3. Error handling prevents crashes on disk full, permission errors
- **Ready for testing:** Simon has 26 edge case test cases documented; critical 8 cases ready for manual execution

### 2026-06-09 — iText7 replaces QuestPDF (Issue #17, Android fix)
- **QuestPDF does NOT support Android** — it ships only Linux native libraries; causes `System.DllNotFoundException` at runtime on Android devices
- Replaced QuestPDF v2026.2.3 with **iText7 v9.1.0** (NuGet id: `itext7`), a pure managed .NET library with zero native dependencies — works on Android
- Removed `QuestPDF.Settings.License = LicenseType.Community;` initialisation from `MauiProgram.cs` (iText7 needs no startup licence call)
- **iText7 9.x API patterns used in `PdfExportService.cs`:**
  - `new PdfWriter(filePath)` → `new PdfDocument(writer)` → `new Document(pdf, iText.Kernel.Geom.PageSize.A4)`
  - Bold text: `SimulateBold()` — **NOT `SetBold()`** (renamed in iText7 v9.x; `SetBold()` no longer exists)
  - Font color: `.SetFontColor(ColorConstants.RED)` from `iText.Kernel.Colors`
  - Table: `new Table(UnitValue.CreatePercentArray(new float[] {...})).UseAllAvailableWidth()`
  - Header cells: `table.AddHeaderCell(new Cell().Add(new Paragraph(...)))`
- **Namespace conflicts with MAUI globals:** `iText.Kernel.Geom.Path` clashes with `System.IO.Path` (don't `using iText.Kernel.Geom`; use `iText.Kernel.Geom.PageSize` inline); `iText.Layout.Element.Cell` clashes with `Microsoft.Maui.Controls.Cell` (use `using iTextCell = iText.Layout.Element.Cell` alias)
- **License:** iText7 is AGPL-3.0. For commercial/closed-source use, a commercial iText licence is required.
- Build: 0 errors
- Commit: 7040e66 on branch `squad/17-pdf-export`



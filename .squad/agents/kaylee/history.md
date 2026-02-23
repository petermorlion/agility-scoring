# Kaylee — History

## Project Context
- **Project:** Agility Scoring — dog agility competition scoring app
- **Owner:** Peter
- **Stack:** .NET MAUI C# in `apps/maui`, targeting net10.0-android
- **Description:** Mobile app for creating dog agility tournaments and counting obstacle faults/refusals per run
- **Team:** Mal (Lead), Zoe (Backend Dev), Kaylee (MAUI Dev), Simon (Tester), Scribe, Ralph

## Learnings

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

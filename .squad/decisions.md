# Decisions

> Authoritative record of team decisions. Append-only. Managed by Scribe.

<!-- Scribe merges from .squad/decisions/inbox/ into this file. Do not edit manually. -->

## Auth Decisions

### 2026-02-23T20:19:14+01:00: Auth: Google OAuth via WebAuthenticator
**By:** Peter (via Kaylee)  
**What:** Replace MSAL with Google OAuth using MAUI WebAuthenticator + PKCE flow. No Entra/Azure AD.  
**Why:** App needs Google logins, not Microsoft identity. MSAL was wrong provider.

### 2026-06-09: Google OAuth — Migrate to Reverse Client ID URI Scheme
**By:** Kaylee  
**Status:** ✅ Implemented  
**What:** Change Android OAuth redirect URI from arbitrary custom scheme (`agility-scoring-maui://`) to reverse client ID scheme (`com.googleusercontent.apps.{client-id}:/oauth2redirect`).  
**Why:** Google deprecated arbitrary custom URI schemes for native Android OAuth. Reverse client ID is explicitly supported and requires zero new NuGet packages.  
**Option rejected:** Native Google Sign-In SDK (`Xamarin.Google.Android.Play.Services.Auth`) — compatibility risk with `net10.0-android` and MAUI 8.x Activity lifecycle wiring.  
**Changes:** `WebAuthCallbackActivity.cs` (DataScheme + DataPath) and `ConfigService.cs` (RedirectUri). Build: 0 errors.  
**Action for Peter:** Verify Google Cloud Console credential type for `650791542042-6ub4916cfv65tt54566ecedu6qkaqgti`. If Web credential, add reverse URI to Authorised Redirect URIs.

## Deployment Decisions

### 2026-02-23: Android Deployment — Device ADB Connection
**By:** Kaylee  
**Status:** ⚠️ Blocked (awaiting Peter)  
**What:** MAUI app deployment to physical Android device requires proper ADB authorization.  
**Why:** Device must show as "device" (authorized) in `adb devices`, not "offline" or "unauthorized".  
**Build status:** 0 errors, 114 warnings (non-blocking). Successfully compiles for `net10.0-android`.  
**Next steps for Peter:** (1) Enable USB debugging on phone, (2) Accept ADB authorization prompt, (3) Verify device shows as "device", (4) Re-run `dotnet build -t:Run -f net10.0-android`.

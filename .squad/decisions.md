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

## Feature Decisions

### 2026-03-13: Disqualified Button (Issue #18) — API Design
**By:** Zoe (Backend Dev)  
**Status:** ✅ Implemented  
**What:** Implemented `disqualified` as a boolean field on result documents with explicit state mutation (not toggle).  
**Why:** 
- Idempotent: Client can retry safely without side effects
- Clear intent: Desired state is explicit in every call
- Race-safe: Multiple clients won't accidentally flip state
- Simpler: Client manages UI state, server stores it
**Design:**
- Added `disqualified: boolean` field to result documents (default: `false`)
- New `setDisqualified` mutation: `{ id: string, tournamentId: string, disqualified: boolean }`
- Validates result exists and belongs to tournament before updating
- Returns `{ success: boolean, result?: Document, error?: string }`
- Backward compatible: `getResult` returns `false` if field missing on legacy docs
**Technical:** Follows existing `results.ts` patterns, uses Zod validation, MongoDB `$set` operator for atomic updates. All procedures use `publicProcedure` (no auth yet).  
**PR:** #19 (branch `squad/18-disqualified-button`)  
**Note:** MAUI will need mapping logic to convert `contestantNumber` → result `id` for API calls.

### 2026-03-13: Disqualified Button (Issue #18) — MAUI Implementation
**By:** Kaylee (MAUI Dev)  
**Status:** ✅ Implemented (Local Storage Only)  
**What:** Added toggle button to `TournamentDetailPage` for marking contestants as disqualified with local persistence.  
**UI Design:**
- Button: Full-width, below contestant navigation bar
- Text: Dynamic ("Mark as Disqualified" / "Disqualified ✓")
- Colors: Gray (#6c757d) when not disqualified, red (#dc3545) when disqualified
- Visual feedback: `BoolToDisqualifiedColorConverter` for button color binding
**Implementation:** 
- ViewModel properties: `IsDisqualified`, `DisqualifiedButtonText`, `ToggleDisqualifiedCommand`
- LocalStorage via `TournamentDto.ContestantDisqualified` dictionary (keyed by contestant number)
- State loads when contestant number changes; persists immediately
- Error handling: Rollback on failure with user alert
- Backward compatibility: Gracefully handles missing field in existing tournaments (defaults to false)
**Build:** 0 errors, 41 warnings (pre-existing nullability issues).  
**Limitation:** Currently local-only; API integration deferred to separate task.  
**Future:** Will need to call `POST /trpc/setDisqualified` and map `contestantNumber` → result `id`.

### 2026-03-13: Disqualified Button (Issue #18) — Test Strategy
**By:** Simon (Tester)  
**Status:** ✅ Tests Written (Awaiting MongoDB Execution)  
**What:** Created comprehensive test suite for `setDisqualified` tRPC mutation using Node.js built-in test runner.  
**Test Coverage:** 7 test cases
1. Set disqualified to true (happy path)
2. Un-disqualify (toggle undo)
3. Invalid tournamentId (error case)
4. Non-existent result ID (error case)
5. Zod validation (missing fields, type checking)
6. Multiple toggle operations (idempotency)
7. Preserve other result fields (data integrity)
**Infrastructure:**
- File: `apps/api/src/__tests__/setDisqualified.test.ts` (290 lines)
- Script: Added `"test": "node --test --require tsx/cjs src/__tests__/**/*.test.ts"` to `apps/api/package.json`
- Requirements: MongoDB running, Node.js v24+
**Key Finding:** Design discrepancy — API uses result `id` (UUID) but MAUI tracks by `contestantNumber`. Requires mapping layer for integration.  
**Edge Cases Identified:** 7 additional cases identified but not in test scope (race conditions, concurrent updates, negative contestant numbers, DB failures). Documented with recommendations.  
**Next:** Tests ready to execute once MongoDB available. Expected: All 7 should pass.

## Deployment Decisions

### 2026-02-23: Android Deployment — Device ADB Connection
**By:** Kaylee  
**Status:** ⚠️ Blocked (awaiting Peter)  
**What:** MAUI app deployment to physical Android device requires proper ADB authorization.  
**Why:** Device must show as "device" (authorized) in `adb devices`, not "offline" or "unauthorized".  
**Build status:** 0 errors, 114 warnings (non-blocking). Successfully compiles for `net10.0-android`.  
**Next steps for Peter:** (1) Enable USB debugging on phone, (2) Accept ADB authorization prompt, (3) Verify device shows as "device", (4) Re-run `dotnet build -t:Run -f net10.0-android`.

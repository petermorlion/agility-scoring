# Decisions

> Authoritative record of team decisions. Append-only. Managed by Scribe.

<!-- Scribe merges from .squad/decisions/inbox/ into this file. Do not edit manually. -->

## Auth Decisions

### 2026-02-23T20:19:14+01:00: Auth: Google OAuth via WebAuthenticator
**By:** Peter (via Kaylee)  
**What:** Replace MSAL with Google OAuth using MAUI WebAuthenticator + PKCE flow. No Entra/Azure AD.  
**Why:** App needs Google logins, not Microsoft identity. MSAL was wrong provider.

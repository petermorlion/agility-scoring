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

# Scribe — History

## Project Context
- **Project:** Agility Scoring — dog agility competition scoring app
- **Owner:** Peter
- **Stack:** Turborepo monorepo — Node.js/Express/tRPC/MongoDB (`apps/api`) + .NET MAUI C# (`apps/maui`)
- **Team:** Mal (Lead), Zoe (Backend Dev), Kaylee (MAUI Dev), Simon (Tester), Scribe, Ralph

## Learnings

### PDF Library Selection for Android (2026-03-13)
When selecting PDF generation libraries for .NET MAUI Android targets (`net10.0-android`):
- **Requirement:** Pure managed .NET only — APK packaging rejects native Linux/Windows dependencies
- **QuestPDF:** Native Linux libraries (`.so`) — **NOT viable** for Android despite excellent DX
- **iText7:** High-level table API, BUT requires `itext.bouncy-castle-adapter` (native runtime failures on Android) — **NOT viable**
- **PDFsharp v6.2.0:** Pure managed .NET, MIT license, low-level XGraphics API (verbose but fully functional on Android) — **VIABLE**
- **Android font handling:** System fonts (Arial) unavailable via PDFsharp; use Helvetica (PDFsharp fallback)
- **Takeaway:** Prefer API quality over developer experience when native dependencies are a deal-breaker for deployment targets

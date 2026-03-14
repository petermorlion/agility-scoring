# Decision: Disqualified Localization Strategy

**Date:** 2025-01-12  
**By:** Kaylee (MAUI Dev)  
**Status:** ✅ Implemented

## Decision
Localize disqualified text and PDF headers using separate localization keys for button text vs. PDF acronym.

## Context
The disqualified feature had hardcoded English strings in two places:
1. `TournamentDetailViewModel.DisqualifiedButtonText` — button label that toggles disqualified status
2. `PdfExportService.ExportTournamentToPdfAsync()` — PDF column header and per-row "DQ" indicator

## Options Considered

### Option 1: Single localization key for all disqualified text
- Use one key like `Disqualified` for both button and PDF
- **Rejected:** Button needs full phrase ("Mark as Disqualified" vs "Disqualified ✓"), PDF column header needs short acronym ("DQ" / "DK") to fit narrow column

### Option 2: Separate keys for button and PDF ✅ (chosen)
- `MarkAsDisqualified` — button label when not disqualified
- `DisqualifiedResult` — button label when disqualified
- `DQAcronym` — short acronym for PDF column header and per-row indicator
- **Why:** Different UI contexts require different formats (full phrase vs. 2-letter acronym)

## Implementation
- Added 8 new keys to `LocalizationService.cs` (3 for disqualified, 5 for PDF column headers) across all 4 languages (English, Français, Deutsch, Nederlands)
- Updated `TournamentDetailViewModel.DisqualifiedButtonText` to use localization service indexer
- Injected `LocalizationService` into `PdfExportService` via constructor (DI already wired)
- Replaced all hardcoded PDF column headers with localized keys

## Key Translations
| Key | English | Français | Deutsch | Nederlands |
|-----|---------|----------|---------|------------|
| `DQAcronym` | DQ | DQ | DQ | **DK** |

Note: Dutch uses "DK" (gediskwalificeerd) instead of "DQ" to match local convention.

## Build Result
0 errors, exit code 0, 87 seconds

## Commit
867fa4c on `main`

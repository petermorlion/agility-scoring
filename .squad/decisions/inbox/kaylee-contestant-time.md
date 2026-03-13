# Decision: Contestant Run Time Input Design

**Date:** 2025-01-11  
**Author:** Kaylee (MAUI Dev)  
**Status:** Implemented

## Context

Users need to record the time it takes each contestant to complete their run. This is a critical metric for agility competitions alongside faults and refusals.

## Decision

Implemented a simple minutes + seconds input field pattern with immediate auto-save persistence.

### Design Choices

1. **Two separate Entry fields (minutes + seconds)** instead of a single time picker or formatted text entry
   - **Rationale:** Simple numeric keyboard input is faster for mobile users during live scoring; no need to switch keyboard types or parse formatted strings
   - **Trade-off:** Takes slightly more horizontal space but provides clearer UX

2. **Immediate auto-save on property change** (mirroring disqualified toggle pattern)
   - **Rationale:** Consistency with existing features; eliminates need for "Save" button; prevents data loss
   - **Trade-off:** Generates more writes to LocalStorage but acceptable given small data size

3. **Dictionary-based per-contestant state management** (`_contestantTimes` keyed by contestant number)
   - **Rationale:** Matches the existing patterns for `_contestantDisqualified` and `_contestantNames`; single source of truth in ViewModel
   - **Trade-off:** None; this is the established pattern

4. **Time format in PDF: "M:SS"** (e.g., "2:45" not "02:45")
   - **Rationale:** Standard sports time format; saves horizontal space in PDF column
   - **Default:** "0:00" when time not set

5. **No validation on minutes/seconds** (allows any integer)
   - **Rationale:** Simple implementation; MAUI Entry with Keyboard="Numeric" already prevents non-numeric input
   - **Future consideration:** Add max seconds validation (e.g., seconds < 60) if users request it

### PDF Column Width Adjustments

Original widths (5 columns, 495 total):
- # (40), Name (200), Refusals (80), Faults (80), DQ (95)

New widths (6 columns, 495 total):
- # (35), Name (155), Time (65), Refusals (70), Faults (70), DQ (100)

**Rationale:** Time column needs ~65 points for "MM:SS" format; reduced Name column from 200→155 to compensate; all columns still readable

## Alternatives Considered

1. **Single formatted text entry (MM:SS)**
   - Rejected: Requires string parsing, validation, and custom keyboard handling
   - Would need regex validation and error messages for invalid formats

2. **Time picker control**
   - Rejected: MAUI TimePicker is for clock times (hours:minutes), not durations; overkill for simple minute/second entry

3. **Deferred save with explicit "Save" button**
   - Rejected: Inconsistent with disqualified toggle's auto-save pattern; adds unnecessary UI complexity

## Impact

- **Files changed:** 5 (TournamentDetailPage.xaml, TournamentDetailViewModel.cs, LocalStorageService.cs, PdfExportService.cs, TournamentListViewModel.cs)
- **Data migration:** None required; new `ContestantTimes` dictionary is optional in `TournamentDto` and defaults gracefully
- **Testing:** Build succeeded; manual testing required for time input UX and PDF rendering

## Future Considerations

- Consider adding seconds validation (seconds < 60) if users accidentally enter invalid values
- Could add a "Clear Time" button if users want to reset to 0:00 quickly
- Milliseconds precision not needed for current use case but could be added if requested

## Cross-Team Notes

- **Zoe (Backend):** When API integration is added, time values will need to map to contestant runs (similar to disqualified status)
- **Simon (Testing):** New edge cases for PDF export: time column rendering, default "0:00" display, column width overflow with long names

# Simon — History

## Project Context
- **Project:** Agility Scoring — dog agility competition scoring app
- **Owner:** Peter
- **Stack:** Turborepo monorepo — Node.js/Express/tRPC/MongoDB (`apps/api`) + .NET MAUI C# (`apps/maui`)
- **Description:** Mobile app for creating dog agility tournaments and counting obstacle faults/refusals per run
- **Team:** Mal (Lead), Zoe (Backend Dev), Kaylee (MAUI Dev), Simon (Tester), Scribe, Ralph

## Learnings

### 2026-03-13: Test Infrastructure Setup for Disqualified Button (Issue #18)
**What:** Created comprehensive test suite for `setDisqualified` tRPC mutation using Node.js built-in test runner.

**Key learnings:**
- Project uses Node v24.12.0, which has native test runner (`node:test` module) — no Jest/Mocha needed
- No existing test infrastructure was present in `apps/api/` — first test file created
- Tests written TDD-style before implementation to guide backend development
- MongoDB test database pattern: connect to separate `agility_scoring_test` db to avoid polluting production data
- tRPC router testing via `appRouter.createCaller({})` for direct procedure invocation

**Test coverage created:**
1. Set disqualified to true (happy path)
2. Set disqualified to false / un-disqualify (toggle undo)
3. Invalid tournamentId (error case)
4. Non-existent contestant number (error case)
5. Zod validation for all required fields + type checking
6. Multiple toggle operations (edge case)
7. Preserve other result fields during update (data integrity)

**Edge cases identified but NOT tested:**
- Negative/zero contestant numbers (needs Zod `.positive()` validation)
- Concurrent updates (race conditions)
- Backward compatibility with results missing `disqualified` field
- MAUI UI debouncing and error handling (requires UI tests)

**Files created:**
- `apps/api/src/__tests__/setDisqualified.test.ts` (7 test cases)
- `.squad/decisions/inbox/simon-disqualified.md` (implementation notes for Zoe)
- `.squad/decisions/inbox/simon-disqualified-test-cases.md` (detailed edge case analysis)

**package.json changes:**
- Added `"test": "node --test --require tsx/cjs src/__tests__/**/*.test.ts"` script

**Next steps:**
- Zoe implements `setDisqualified` endpoint in `results.ts` ✅ DONE
- Tests should pass after implementation
- Kaylee wires up MAUI toggle button to call endpoint (currently local-only) ✅ DONE — awaiting API integration
- Simon reviews implementation and runs tests — awaiting MongoDB availability

**Cross-team context:**
- Zoe implemented API with explicit state mutation design (not toggle) — idempotent and race-safe
- Kaylee built local-only MAUI UI; no API integration yet
- Key finding: Design discrepancy between API (uses result `id`) and MAUI (uses `contestantNumber`) — integration will require mapping layer
- Documented recommendations for backend (add validation, ensure backward compatibility) and frontend (disable button during API call, error handling)

### 2026-03-13: PDF Export Test Case Analysis (Issue #17)
**What:** Documented comprehensive test cases for PDF export feature — tournament results to PDF with contestant scores, disqualified status, and totals.

**Key learnings:**
- No MAUI test project exists — all testing will be manual until infrastructure created
- QuestPDF v2026.2.3 already added to AgilityScoring.Maui.csproj (line 55)
- MAUI uses local-only data model (TournamentDto from LocalStorageService) with no API integration yet
- Data model stores contestant data in dictionaries keyed by contestant number (not result ID like API)
- Obstacle keys discrepancy found: Backend canonical enum has 7 obstacles (aframe, dogwalk, seesaw, tunnel, chute, jump, tire), but TournamentDetailViewModel shows 9 (includes weave, teeter, table)

**Test coverage documented:** 26 edge cases across 6 categories
1. **Data model variations (12 cases):** Empty tournament, single contestant, default names, disqualified status, zero scores, max values, special characters, long names, invalid dates, large datasets, sparse data
2. **File system errors (5 cases):** Insufficient storage, missing permissions, file race conditions, rapid taps/debouncing, background task interruption
3. **Performance (2 cases):** 100 contestants, 500 contestants (stress test)
4. **Compatibility (2 cases):** PDF viewer compatibility, QuestPDF version across Android API levels
5. **MAUI ViewModel/UI (4 cases):** Command binding, success feedback, failure feedback, loading states
6. **Data integrity (1 case):** No data mutation during export

**Critical test cases (must-test):** 8 cases identified
- Empty tournament (case 1)
- Disqualified contestant display (case 5)
- Special characters in name (case 8)
- Insufficient storage (case 13)
- Multiple rapid taps/debounce (case 16)
- Export success feedback (case 22)
- Export failure feedback (case 23)

**Files created:**
- `.squad/decisions/inbox/simon-pdf-export-test-cases.md` (26 test cases with expected behaviors)
- `.squad/decisions/inbox/simon-pdf-export.md` (findings, recommendations, implementation guidance for Kaylee)

**Recommendations for Kaylee:**
1. Service interface: `IPdfExportService.ExportTournamentToPdfAsync(TournamentDto) -> Result<string>`
2. File naming: `Tournament_{Name}_{Date}.pdf` with sanitized special characters
3. Error handling: Catch IOException (storage), OutOfMemoryException (too large), generic fallback
4. Totals calculation: Sum all ObstacleScore.Refusals and Faults across contestant's obstacles
5. Default values: Missing name -> "Contestant N", missing disqualified -> false, missing scores -> 0
6. Debouncing: Use `IsBusy` check in CanExecute for export command
7. User feedback: Toast/alert with file path + "Open PDF" action
8. Performance: Use `Task.Run()` for background thread if >50 contestants

**Open questions for team:**
1. Obstacle keys discrepancy — which set is authoritative? (Backend 7 vs MAUI 9)
2. PDF layout — sort by contestant number or by leaderboard (total faults)?
3. File sharing — include "Share PDF" button?
4. Localization — should PDF labels be translated via LocalizationService?
5. QuestPDF license compliance — is project commercial or open-source?

**Test infrastructure recommendation:**
- Create `apps/maui/AgilityScoring.Maui.Tests` project with xUnit
- Write unit tests for PdfExportService (file path returned, file exists, handles empty tournament)
- Write ViewModel tests for ExportTournamentToPdfCommand (success/error paths, IsBusy state)
- Enables CI/CD automation and regression prevention

**Next steps:**
1. Kaylee implements PdfExportService with QuestPDF
2. Kaylee adds ExportTournamentToPdfCommand to TournamentListViewModel
3. Kaylee adds Export button to TournamentListPage.xaml (one per tournament box)
4. Simon manually tests 8 critical cases after implementation
5. Optional: Create MAUI test project for automated ViewModel tests


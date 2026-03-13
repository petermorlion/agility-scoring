# Decision: PDF Export Implementation

**Date:** 2026-03-13  
**Author:** Kaylee (MAUI Dev)  
**Issue:** #17 — PDF export for tournaments  
**Status:** Implemented

## Context
Users need to export tournament results to PDF format for printing or sharing. The PDF should include tournament details (name, date) and a table of contestant results (contestant number, name, refusals, faults, disqualified status).

## Decision

### Library Choice: QuestPDF
- **Selected:** QuestPDF (NuGet package `QuestPDF`, version 2026.2.3)
- **License:** MIT / Community (free for open source)
- **Rationale:** 
  - Native .NET library with fluent API
  - Works well on Android (MAUI target)
  - No external dependencies or native bindings
  - Produces high-quality PDFs with table layouts
  - Actively maintained and well-documented

### Architecture
- **Service:** `PdfExportService` — stateless service for generating PDFs
- **Location:** `apps/maui/Services/PdfExportService.cs`
- **DI Registration:** Singleton in `MauiProgram.cs`
- **Data model:** `ContestantResult` — flattened DTO with aggregated scores
  - `ContestantNumber` (int)
  - `Name` (string)
  - `TotalRefusals` (int) — sum of all obstacle refusals
  - `TotalFaults` (int) — sum of all obstacle faults
  - `IsDisqualified` (bool) — read from tournament's `ContestantDisqualified` dictionary

### PDF Content
- **Header:** Tournament name (24pt bold), date (16pt)
- **Table columns:** #, Name, Refusals, Faults, DQ
- **Sorting:** Contestants ordered by contestant number (ascending)
- **DQ indicator:** Red "DQ" text if disqualified, empty cell otherwise
- **Page size:** A4 with 2cm margins

### File Handling
- **Save location:** `FileSystem.CacheDirectory` (temporary storage)
- **Filename format:** `{TournamentName}_{yyyyMMdd_HHmmss}.pdf` (with spaces replaced by underscores)
- **Opening:** `Launcher.OpenAsync()` with `OpenFileRequest` — delegates to system PDF viewer/share sheet

### UI Integration
- **Location:** TournamentListPage — export button in each tournament card
- **Button:** "📄 PDF" — 80px wide, primary color, positioned at right of card
- **Layout:** Grid with two columns — tournament info (left), export button (right, vertically centered)
- **Command:** `ExportToPdfCommand` in `TournamentListViewModel`
- **Loading state:** Uses `IsBusy` to prevent duplicate exports

### Technical Challenges Resolved
1. **Namespace conflicts:**
   - QuestPDF has `IContainer` type that conflicts with `Microsoft.Maui.IContainer`
   - QuestPDF has `Colors` type that conflicts with `Microsoft.Maui.Graphics.Colors`
   - **Solution:** Fully qualify types: `QuestPDF.Infrastructure.IContainer`, `QuestPDF.Helpers.Colors`

2. **QuestPDF license requirement:**
   - Community edition requires explicit license declaration
   - **Solution:** Added `QuestPDF.Settings.License = LicenseType.Community;` in `MauiProgram.CreateMauiApp()` before building the app

3. **Data aggregation:**
   - Tournament data stored as `ContestantScores` (dictionary of dictionaries: `int → string → ObstacleScore`)
   - **Solution:** Loop through contestant's obstacles, sum up `Refusals` and `Faults` values

## Alternatives Considered
1. **PdfSharp / MigraDoc** — more verbose API, heavier library
2. **SkiaSharp** — would require manual layout, no table primitives
3. **HTML → PDF (WebView)** — dependency on WebView rendering, platform inconsistencies

## Impact
- **Files changed:**
  - `apps/maui/AgilityScoring.Maui.csproj` — added QuestPDF package reference
  - `apps/maui/MauiProgram.cs` — registered service, set license
  - `apps/maui/Services/PdfExportService.cs` — new file
  - `apps/maui/ViewModels/TournamentListViewModel.cs` — added export command, injected service
  - `apps/maui/Views/TournamentListPage.xaml` — added export button to card layout

- **Build:** Successful (0 errors, pre-existing warnings unchanged)

## Future Considerations
- If many contestants, consider pagination (currently single-page table)
- Could add tournament logo/branding to header
- Could include obstacle-by-obstacle breakdown (currently only totals)
- API integration: when switching to server-based storage, export logic remains unchanged (just fetch data from API instead of LocalStorage)

## Related Decisions
- Builds on #18 (Disqualified button) — DQ status integrated into PDF
- Uses LocalStorageService (no API integration yet)

# PDF Export Implementation — Findings & Recommendations

**Issue:** #17 PDF Export  
**Tester:** Simon  
**Date:** 2026-03-13  
**Status:** Test cases documented, awaiting implementation

---

## Summary

Analyzed PDF export requirements for tournament results. Documented 26 edge cases covering data model variations, file system errors, UI/UX concerns, and performance considerations.

**Key Finding:** No MAUI test project exists. All testing will be manual until infrastructure is created.

---

## Data Model Analysis

From `apps/maui/Services/LocalStorageService.cs`:

```csharp
TournamentDto {
  string Id                                                    // UUID
  string Name                                                  // Tournament name
  string Date                                                  // ISO date string
  Dictionary<int, string> ContestantNames                      // contestantNumber -> name
  Dictionary<int, Dictionary<string, ObstacleScore>> ContestantScores
  Dictionary<int, bool> ContestantDisqualified                 // contestantNumber -> bool
}

ObstacleScore {
  int Refusals
  int Faults
}
```

**Obstacle keys (canonical):** `aframe | dogwalk | seesaw | tunnel | chute | jump | tire`

**Note:** TournamentDetailViewModel shows additional obstacles (`weave`, `teeter`, `table`) not in canonical enum. This discrepancy should be verified with backend team.

---

## Critical Edge Cases (Must Test)

### 1. Empty Tournament (0 contestants)
**Risk:** NullReferenceException or crash if no data validation
**Mitigation:** PDF should show "No contestants" message

### 2. Disqualified Contestant Display
**Risk:** Disqualified status not visible or unclear
**Mitigation:** Use clear visual indicator (red text, checkmark, icon)

### 3. Tournament Name Special Characters
**Risk:** PDF encoding issues with apostrophes, dashes, unicode
**Mitigation:** Test with: `National's 2026 — Championship (Round 1)`

### 4. Insufficient Storage Space
**Risk:** Partial file written, no error shown to user
**Mitigation:** Catch IOException, show user-friendly error, cleanup partial file

### 5. Multiple Rapid Export Taps (No Debounce)
**Risk:** Multiple exports run simultaneously, file overwrites, crashes
**Mitigation:** Disable button during export via `IsBusy` check in CanExecute

### 6. Export Success Feedback
**Risk:** User doesn't know where PDF was saved
**Mitigation:** Show toast/alert with file path + "Open PDF" button

### 7. Export Failure Feedback
**Risk:** Silent failure, user thinks export succeeded
**Mitigation:** Show alert with error message, re-enable export button

---

## Recommendations for Kaylee (MAUI Developer)

### Service Design
```csharp
public interface IPdfExportService
{
    Task<Result<string>> ExportTournamentToPdfAsync(TournamentDto tournament);
}

public class Result<T>
{
    public bool Success { get; set; }
    public T Value { get; set; }
    public string Error { get; set; }
}
```

### File Naming Convention
Format: `Tournament_{Name}_{Date}.pdf`  
Sanitization: Remove `/`, `:`, `*`, `?`, `"`, `<`, `>`, `|` from filename

### File Location
- Android: Use `FileSystem.AppDataDirectory` (no permissions needed)
- Or: Use file picker if scoped storage required (Android 11+)

### Error Handling
Catch specific exceptions:
- `IOException` -> "Storage full or permission denied"
- `OutOfMemoryException` -> "Too many contestants to export"
- `Exception` -> "Export failed. Please try again."

### Totals Calculation
For each contestant:
```csharp
var totalRefusals = tournament.ContestantScores[contestantNum]
    .Values.Sum(score => score.Refusals);
var totalFaults = tournament.ContestantScores[contestantNum]
    .Values.Sum(score => score.Faults);
```

### Default Values
- Missing `ContestantNames[num]` -> `"Contestant {num}"`
- Missing `ContestantDisqualified[num]` -> `false`
- Missing `ContestantScores[num]` -> treat as 0 refusals, 0 faults

### ViewModel Integration (TournamentListViewModel)
Add command:
```csharp
[RelayCommand(CanExecute = nameof(CanExport))]
private async Task ExportTournamentToPdf(TournamentDto tournament)
{
    if (IsBusy) return;
    
    try
    {
        IsBusy = true;
        var result = await _pdfExportService.ExportTournamentToPdfAsync(tournament);
        
        if (result.Success)
        {
            await Shell.Current.DisplayAlert("Success", 
                $"PDF exported to {result.Value}", "OK");
            // Optional: await Launcher.OpenAsync(new OpenFileRequest { ... });
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", result.Error, "OK");
        }
    }
    catch (Exception ex)
    {
        await Shell.Current.DisplayAlert("Error", 
            "Export failed. Please try again.", "OK");
    }
    finally
    {
        IsBusy = false;
    }
}

private bool CanExport() => !IsBusy;
```

### UI Button (TournamentListPage.xaml)
Add to each tournament box in CollectionView:
```xml
<Button 
    Text="Export to PDF"
    Command="{Binding Source={RelativeSource AncestorType={x:Type viewmodels:TournamentListViewModel}}, Path=ExportTournamentToPdfCommand}"
    CommandParameter="{Binding .}"
    Style="{StaticResource SecondaryButtonStyle}" />
```

### Performance Consideration
If tournament has >50 contestants, run PDF generation on background thread:
```csharp
await Task.Run(() => GeneratePdfDocument(tournament));
```

---

## Test Infrastructure Status

**MAUI Test Project:** NOT FOUND  
**Searched patterns:** `**/*.Test.csproj`, `**/*Tests.csproj`  
**Result:** No test projects exist for the MAUI app

**Recommendation:** Create `apps/maui/AgilityScoring.Maui.Tests` project using xUnit or NUnit:
```bash
cd apps/maui
dotnet new xunit -n AgilityScoring.Maui.Tests -f net10.0
cd AgilityScoring.Maui.Tests
dotnet add reference ../AgilityScoring.Maui.csproj
dotnet add package Moq
```

**Rationale:** Unit tests for ViewModels and Services would:
1. Prevent regressions during refactoring
2. Document expected behavior
3. Speed up development (no manual testing per change)
4. Enable CI/CD test automation

**Example test (PdfExportService):**
```csharp
[Fact]
public async Task ExportTournamentToPdfAsync_WithValidTournament_ReturnsFilePath()
{
    // Arrange
    var service = new PdfExportService();
    var tournament = new TournamentDto
    {
        Id = Guid.NewGuid().ToString(),
        Name = "Test Tournament",
        Date = "2026-03-13",
        ContestantNames = new() { { 1, "Dog One" } },
        ContestantScores = new() { { 1, new() } }
    };
    
    // Act
    var result = await service.ExportTournamentToPdfAsync(tournament);
    
    // Assert
    Assert.True(result.Success);
    Assert.NotNull(result.Value);
    Assert.True(File.Exists(result.Value));
}
```

---

## Test Execution Plan

**Phase 1: Manual Testing (by Simon)**
- Test cases: 1, 2, 5, 8, 13, 16, 22, 23 (critical path)
- Environment: Android emulator or physical device
- Checklist: Use test cases document as manual test script

**Phase 2: Stress Testing (optional)**
- Test cases: 11 (100 contestants), 18 (500 contestants)
- Measure: Export time, memory usage, file size

**Phase 3: Compatibility Testing (optional)**
- Test case: 19 (PDF opens in multiple viewers)
- Test case: 20 (QuestPDF on different Android versions)

**Phase 4: Automated Tests (future)**
- Create MAUI test project
- Write unit tests for PdfExportService
- Write ViewModel tests for TournamentListViewModel.ExportTournamentToPdfCommand

---

## Dependencies

**NuGet Package:** QuestPDF v2026.2.3 (already installed in AgilityScoring.Maui.csproj line 55)  
**License:** QuestPDF Community License (free for open-source/non-commercial projects)  
**Documentation:** https://www.questpdf.com/

**Verify license compliance:** If agility-scoring is commercial, may need QuestPDF Professional License.

---

## Open Questions

1. **Obstacle keys discrepancy:** Backend uses 7 obstacles (aframe, dogwalk, seesaw, tunnel, chute, jump, tire), but MAUI TournamentDetailViewModel shows 9 (includes weave, teeter, table). Which is correct?

2. **PDF layout:** Should results be sorted by contestant number? Or by total faults (leaderboard style)?

3. **File sharing:** Should export include "Share PDF" button to share via email/messaging?

4. **Localization:** Should PDF labels (Name, Refusals, Faults, Disqualified) be localized using LocalizationService?

5. **Background persistence:** If user backgrounds app during export, should export continue or cancel?

---

## Next Steps

1. **Kaylee:** Implement PdfExportService using QuestPDF
2. **Kaylee:** Add ExportTournamentToPdfCommand to TournamentListViewModel
3. **Kaylee:** Add Export button to TournamentListPage.xaml
4. **Simon:** Manual test critical cases 1, 2, 5, 8, 13, 16, 22, 23
5. **Scribe:** Merge test cases document into decisions.md after review
6. **Optional:** Create MAUI test project for future automation

---

## References

- GitHub Issue: #17
- Test Cases: `.squad/decisions/inbox/simon-pdf-export-test-cases.md`
- Data Model: `apps/maui/Services/LocalStorageService.cs`
- QuestPDF Docs: https://www.questpdf.com/documentation/getting-started.html

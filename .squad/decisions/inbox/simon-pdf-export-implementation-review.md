# PDF Export Implementation Review — Simon's Analysis

**Issue:** #17 PDF Export  
**Reviewer:** Simon (Tester)  
**Implementation by:** Kaylee (MAUI Dev)  
**Date:** 2026-03-13  
**Status:** Implementation in progress — review and test recommendations

---

## Implementation Review

Reviewed `apps/maui/Services/PdfExportService.cs` — Kaylee has implemented a solid foundation for PDF export using QuestPDF.

### ✅ What's Working Well

1. **Async/Background Execution:** Uses `Task.Run()` to avoid UI blocking — good practice
2. **File Naming:** Includes timestamp (`yyyyMMdd_HHmmss`) to prevent overwrites
3. **Table Layout:** Clear 5-column layout (Number, Name, Refusals, Faults, DQ)
4. **Visual Indication:** Disqualified status shows "DQ" in red text — clear visual signal
5. **Sorting:** Results ordered by contestant number — predictable order
6. **Data Model:** Uses `ContestantResult` DTO to decouple service from LocalStorage structure

### ⚠️ Issues to Address

#### 1. File Location: CacheDirectory vs. Persistent Storage
**Current:** `FileSystem.CacheDirectory`  
**Problem:** Android can delete cache files at any time (low storage, app cache clear)  
**Risk:** User exports PDF, app cache cleared, PDF is gone

**Recommendation:**
```csharp
var filePath = Path.Combine(FileSystem.AppDataDirectory, "exports", fileName);
// Or use shared storage:
// var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
```

**Test case:** Export PDF, clear app cache, verify PDF still exists ❌ WILL FAIL

---

#### 2. File Name Sanitization: Incomplete
**Current:** `tournament.Name.Replace(" ", "_")`  
**Problem:** Only replaces spaces, not other invalid filename characters  
**Risk:** Tournament name "Team's Finals: 2026 (Round 1/2)" -> invalid filename

**Recommendation:**
```csharp
private static string SanitizeFileName(string fileName)
{
    var invalid = Path.GetInvalidFileNameChars();
    return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
}

var fileName = $"{SanitizeFileName(tournament.Name)}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
```

**Test case 8:** Tournament name with apostrophes, colons, slashes ❌ WILL FAIL

---

#### 3. Error Handling: Missing
**Current:** No try/catch around PDF generation  
**Problem:** Disk full, permission errors, QuestPDF exceptions -> app crash  
**Risk:** Silent failures, poor user experience

**Recommendation:**
```csharp
public async Task<(bool Success, string FilePath, string Error)> ExportTournamentToPdfAsync(
    TournamentDto tournament, List<ContestantResult> contestants)
{
    try
    {
        return await Task.Run(() =>
        {
            // ... existing code ...
            return (true, filePath, null);
        });
    }
    catch (IOException ex)
    {
        return (false, null, "Storage full or permission denied");
    }
    catch (OutOfMemoryException ex)
    {
        return (false, null, "Too many contestants to export");
    }
    catch (Exception ex)
    {
        return (false, null, $"Export failed: {ex.Message}");
    }
}
```

**Test cases 13, 22, 23:** Disk full, success feedback, error feedback ❌ WILL FAIL

---

#### 4. Empty Tournament Handling: Not Tested
**Current:** `foreach (var contestant in contestants.OrderBy(...))`  
**Behavior:** If `contestants` is empty list, table will have header but no rows  
**Expected:** Should work, but not explicitly tested

**Recommendation:** Add explicit test case

**Test case 2:** Empty tournament (0 contestants) ⚠️ NEEDS VERIFICATION

---

#### 5. Null Name Handling: Inconsistent with UI
**Current:** `Text(contestant.Name ?? "")`  
**Problem:** Shows blank instead of default "Contestant N" like the UI  
**Risk:** PDF doesn't match UI, confusing to user

**Recommendation:**
```csharp
table.Cell().Element(CellStyle).Text(
    !string.IsNullOrEmpty(contestant.Name) 
        ? contestant.Name 
        : $"Contestant {contestant.ContestantNumber}");
```

**Test case 4:** Contestant with default name ❌ WILL FAIL (shows blank, not "Contestant N")

---

#### 6. Export Directory Creation: Missing
**Current:** Writes to `FileSystem.CacheDirectory` (exists by default)  
**Future:** If switching to `AppDataDirectory/exports/`, directory must be created

**Recommendation:**
```csharp
var exportDir = Path.Combine(FileSystem.AppDataDirectory, "exports");
Directory.CreateDirectory(exportDir); // Idempotent — no-op if exists
var filePath = Path.Combine(exportDir, fileName);
```

---

#### 7. Long Tournament Names: Layout Not Tested
**Current:** No explicit max length or truncation  
**Risk:** 80+ character tournament name may overflow PDF header

**Recommendation:** Test with long name, consider truncation if needed

**Test case 9:** Long tournament name (80+ chars) ⚠️ NEEDS VERIFICATION

---

#### 8. Date Validation: Missing
**Current:** Displays `tournament.Date` directly  
**Problem:** If date is null/empty/invalid, PDF shows blank or "invalid-date"  
**Risk:** Poor user experience

**Recommendation:**
```csharp
var dateText = !string.IsNullOrEmpty(tournament.Date) 
    ? tournament.Date 
    : "(No date)";
col.Item().Text(dateText).FontSize(16);
```

**Test case 10:** Missing or invalid date ⚠️ NEEDS VERIFICATION

---

## Test Execution Plan

### Critical Tests (Must Run After Fixes)

| # | Test Case | Expected Result | Status |
|---|-----------|-----------------|--------|
| 1 | Tournament with 3-5 contestants | PDF generated, all data visible | ⏳ PENDING |
| 2 | Empty tournament (0 contestants) | PDF with "No contestants" or empty table | ⏳ PENDING |
| 4 | Contestant without name | Shows "Contestant N", not blank | ❌ FIX NEEDED |
| 5 | Disqualified contestant | Red "DQ" visible | ✅ IMPLEMENTED |
| 8 | Special characters in name | No encoding errors, filename sanitized | ❌ FIX NEEDED |
| 13 | Disk full / permission error | Error message to user, no crash | ❌ FIX NEEDED |
| 22 | Export success | Toast/alert with file path | ⏳ PENDING (ViewModel) |
| 23 | Export failure | Error message, button re-enabled | ❌ FIX NEEDED |

### Secondary Tests (After Critical Pass)

| # | Test Case | Priority |
|---|-----------|----------|
| 3 | Single contestant | Low (edge case) |
| 6 | Zero scores (clean run) | Medium |
| 7 | Maximum score values (99+) | Medium |
| 9 | Long tournament name | Medium |
| 10 | Invalid date | Medium |
| 11 | 100 contestants (pagination) | Low (stress test) |
| 19 | PDF viewer compatibility | Low (compatibility) |

---

## Recommendations for Kaylee

### Priority 1 (Critical — Before Testing)
1. ✅ Add error handling with try/catch + return tuple `(Success, FilePath, Error)`
2. ✅ Sanitize filename — use `Path.GetInvalidFileNameChars()`
3. ✅ Fix null name handling — show "Contestant N" not blank
4. ✅ Change file location from `CacheDirectory` to `AppDataDirectory/exports/`

### Priority 2 (Important — Before PR)
5. ✅ Add date validation — show "(No date)" if null/empty
6. ✅ Test empty tournament — verify table renders with 0 rows
7. ✅ Update ViewModel to handle error tuple — show alerts for success/failure
8. ⚠️ Update tests document with actual implementation details

### Priority 3 (Nice to Have)
9. ⚠️ Add "Open PDF" action after successful export
10. ⚠️ Test long tournament names — add truncation if needed
11. ⚠️ Consider adding tournament logo/branding to PDF header
12. ⚠️ Add page numbers if >1 page (for 50+ contestants)

---

## ViewModel Integration (TournamentListViewModel)

Need to verify changes to `TournamentListViewModel.cs`:

```csharp
[RelayCommand(CanExecute = nameof(CanExportTournament))]
private async Task ExportTournamentToPdf(TournamentDto tournament)
{
    if (IsBusy) return;
    
    try
    {
        IsBusy = true;
        
        // Build ContestantResult list from tournament
        var contestants = new List<ContestantResult>();
        foreach (var contestantNum in tournament.ContestantNames.Keys)
        {
            var scores = tournament.ContestantScores.GetValueOrDefault(contestantNum, new());
            var totalRefusals = scores.Values.Sum(s => s.Refusals);
            var totalFaults = scores.Values.Sum(s => s.Faults);
            var isDisqualified = tournament.ContestantDisqualified.GetValueOrDefault(contestantNum, false);
            
            contestants.Add(new ContestantResult
            {
                ContestantNumber = contestantNum,
                Name = tournament.ContestantNames.GetValueOrDefault(contestantNum),
                TotalRefusals = totalRefusals,
                TotalFaults = totalFaults,
                IsDisqualified = isDisqualified
            });
        }
        
        var (success, filePath, error) = await _pdfExportService.ExportTournamentToPdfAsync(
            tournament, contestants);
        
        if (success)
        {
            await Shell.Current.DisplayAlert("Success", 
                $"PDF exported to:\n{filePath}", "OK");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", error ?? "Export failed", "OK");
        }
    }
    catch (Exception ex)
    {
        await Shell.Current.DisplayAlert("Error", 
            "Unexpected error during export", "OK");
    }
    finally
    {
        IsBusy = false;
    }
}

private bool CanExportTournament() => !IsBusy;
```

---

## Manual Test Checklist for Simon

Once Priority 1 fixes are complete:

### Setup
1. ✅ Checkout branch `squad/17-pdf-export`
2. ✅ Build MAUI app: `dotnet build -t:Run -f net10.0-android`
3. ✅ Create test tournaments in app:
   - Tournament A: "Normal Tournament" with 3 contestants (mixed scores)
   - Tournament B: "Test's Finals: 2026 (Round 1/2)" (special chars)
   - Tournament C: "Empty Tournament" with 0 contestants
   - Tournament D: Single contestant with no name

### Test Execution
- [ ] Export Tournament A — verify PDF opens, all data correct
- [ ] Export Tournament B — verify filename sanitized, PDF opens
- [ ] Export Tournament C — verify PDF shows empty table or message
- [ ] Export Tournament D — verify PDF shows "Contestant 1", not blank
- [ ] Export with disk nearly full — verify error message (if possible on emulator)
- [ ] Tap export button 3 times rapidly — verify only 1 export runs
- [ ] Navigate away during export — verify no crash
- [ ] Open exported PDF in multiple viewers (Chrome, Google Drive PDF, Adobe if available)

### Pass Criteria
- All 8 tests pass
- No crashes
- Error messages clear and actionable
- PDFs open in all viewers

---

## Next Steps

1. **Kaylee:** Implement Priority 1 fixes (error handling, filename sanitization, name defaults, file location)
2. **Kaylee:** Update ViewModel integration to handle error tuple
3. **Kaylee:** Test locally with critical test cases 1, 2, 4, 8
4. **Kaylee:** Push changes to branch `squad/17-pdf-export`
5. **Simon:** Run full manual test checklist
6. **Simon:** Document test results
7. **Scribe:** Merge implementation into decisions.md after approval
8. **Team:** Close issue #17

---

## References

- GitHub Issue: #17
- Test Cases: `.squad/decisions/inbox/simon-pdf-export-test-cases.md`
- Findings: `.squad/decisions/inbox/simon-pdf-export.md`
- Implementation: `apps/maui/Services/PdfExportService.cs`
- QuestPDF Docs: https://www.questpdf.com/documentation/getting-started.html

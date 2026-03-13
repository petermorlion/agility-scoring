# PDF Export Test Cases — Issue #17

**Feature:** Export tournament results to PDF
**Tester:** Simon
**Date:** 2026-03-13

## Requirements Summary
From GitHub Issue #17:
- Add button in tournament list to export each tournament to PDF
- PDF must contain:
  - Tournament name
  - Tournament date
  - List of results with:
    - Contestant number
    - Contestant name
    - Total refusals count
    - Total faults count
    - Disqualified status (true/false)

## Data Model (from LocalStorageService.cs)
```csharp
TournamentDto {
  string Id
  string Name
  string Date
  Dictionary<int, string> ContestantNames              // key: contestantNumber
  Dictionary<int, Dictionary<string, ObstacleScore>> ContestantScores  // key: contestantNumber
  Dictionary<int, bool> ContestantDisqualified         // key: contestantNumber
}

ObstacleScore {
  int Refusals
  int Faults
}
```

**Obstacle keys used:** `aframe | dogwalk | seesaw | tunnel | chute | jump | tire`
(Note: TournamentDetailViewModel also shows `weave`, `teeter`, `table` — verify which set is correct)

## Test Cases

### 1. Happy Path
**Test:** Export tournament with multiple contestants (3-5)
**Given:** Tournament with 4 contestants, all have names, all have mixed scores (some refusals, some faults)
**Expected:** 
- PDF generated successfully
- File path returned
- File exists at path
- PDF readable (no corruption)
- All contestant data visible in order
- Totals calculated correctly

---

### 2. Empty Tournament (0 Contestants)
**Test:** Export tournament with no contestants
**Given:** Tournament exists with Name and Date, but empty ContestantNames/ContestantScores dictionaries
**Expected:** 
- PDF generated successfully
- Shows tournament name and date
- Shows "No contestants" or empty results table
- No crash or error

---

### 3. Single Contestant
**Test:** Export tournament with exactly 1 contestant
**Given:** Tournament with 1 contestant, full scores
**Expected:** 
- PDF generated successfully
- Single row in results table
- All data renders correctly

---

### 4. Contestant with Default Name
**Test:** Export contestant without custom name (uses default "Contestant N")
**Given:** Tournament with contestant #3 who has no entry in ContestantNames dictionary
**Expected:** 
- PDF shows "Contestant 3" as the name
- Matches the UI behavior from TournamentDetailViewModel.ContestantName

---

### 5. Disqualified Contestant
**Test:** Export tournament with disqualified contestant
**Given:** Tournament with contestant #2 where ContestantDisqualified[2] = true
**Expected:** 
- PDF clearly shows disqualified status (e.g., "✓" or "Yes" in DQ column)
- Disqualified contestant distinguishable from others (e.g., red text, strikethrough, or icon)

---

### 6. Zero Scores (Clean Run)
**Test:** Export contestant with 0 refusals and 0 faults on all obstacles
**Given:** Tournament with contestant who has all ObstacleScore values = { Refusals: 0, Faults: 0 }
**Expected:** 
- PDF shows "0" for both refusals and faults
- No blank cells or missing data

---

### 7. Maximum Score Values
**Test:** Export contestant with extreme scores
**Given:** Tournament with contestant who has 99 refusals and 99 faults on multiple obstacles
**Expected:** 
- PDF renders large numbers correctly
- No text overflow or layout issues
- Totals calculated correctly (e.g., 5 obstacles × 99 = 495)

---

### 8. Tournament Name with Special Characters
**Test:** Export tournament with special characters in name
**Given:** Tournament name = "National's 2026 — Championship (Round 1)"
**Expected:** 
- PDF renders all characters correctly (apostrophes, dashes, parentheses)
- No encoding issues or corrupted characters
- File name sanitized if needed (e.g., replace `/` or `:`)

---

### 9. Long Tournament Name
**Test:** Export tournament with very long name (80+ characters)
**Given:** Tournament name = "International Dog Agility World Championship Finals 2026 - Masters Division Round 3 Qualifier"
**Expected:** 
- PDF handles long text without overflow
- Text wraps or truncates gracefully
- Header remains readable

---

### 10. Missing or Invalid Date
**Test:** Export tournament with null/empty/invalid date
**Given:** Tournament.Date = "" or null or "invalid-date"
**Expected:** 
- PDF generated successfully
- Shows blank date or "(No date)" placeholder
- No crash

---

### 11. Large Number of Contestants (100+)
**Test:** Export tournament with 100 contestants
**Given:** Tournament with ContestantNames[1..100]
**Expected:** 
- PDF generated successfully (may be multi-page)
- All 100 contestants visible across pages
- Page breaks don't split rows awkwardly
- Performance acceptable (<10 seconds)

---

### 12. Contestants with Sparse Data
**Test:** Export tournament where contestant has scores for only 2 obstacles
**Given:** Tournament with contestant who has entries for "jump" and "tunnel" only
**Expected:** 
- PDF shows totals based on obstacles with data
- Missing obstacles treated as 0 or skipped appropriately
- No null reference errors

---

## Edge Cases — File System & UI

### 13. Insufficient Storage Space
**Test:** Export when device storage is nearly full
**Given:** Android device with <1 MB free space
**Expected:** 
- Export fails gracefully with user-friendly error message
- Shows "Not enough storage space" alert
- No crash
- Partial file is not left on disk

---

### 14. Missing Write Permissions
**Test:** Export when app lacks write permissions to Documents folder
**Given:** Simulated permission denial (requires test double or emulator)
**Expected:** 
- Export fails gracefully
- Shows "Permission denied" error to user
- Prompts user to grant storage permission (if possible on Android)

---

### 15. File Already Open (Race Condition)
**Test:** User opens exported PDF while export is still writing
**Given:** Slow write simulation + immediate file open attempt
**Expected:** 
- Either: Export completes before file is opened
- Or: File viewer shows partial data then refreshes (acceptable)
- Or: Error message "File in use" (acceptable)
- No crash or corrupted PDF

---

### 16. Multiple Rapid Export Taps (Debouncing)
**Test:** User taps "Export to PDF" button 5 times rapidly
**Given:** No debounce logic in ViewModel
**Expected:** 
- Either: Only 1 export executes (preferred — button disabled during export)
- Or: Multiple files created with unique names (e.g., timestamp suffixes)
- No crash or duplicate file overwrites

---

### 17. Export During Background Task
**Test:** User backgrounds app (home button) while export is in progress
**Given:** Export takes 5 seconds, user backgrounds after 2 seconds
**Expected:** 
- Export completes in background (preferred)
- Or: Export fails gracefully with user notification
- File is either complete or cleaned up (no partial files)

---

### 18. Large File Size (Memory)
**Test:** Export tournament with 500 contestants (stress test)
**Given:** Tournament with 500 contestants × 7 obstacles = 3500 scores
**Expected:** 
- PDF generated successfully or fails with clear "Out of memory" error
- App does not crash or ANR (Android Not Responding)
- If successful, file opens correctly

---

## Integration & Compatibility

### 19. PDF Viewer Compatibility
**Test:** Exported PDF opens in multiple PDF viewers
**Given:** Export successful, file saved to Documents folder
**Expected:** 
- PDF opens in default Android PDF viewer (Google Drive PDF Viewer)
- PDF opens in Adobe Reader, if installed
- PDF opens in Chrome browser
- No rendering issues across viewers

---

### 20. QuestPDF Version Compatibility
**Test:** Verify QuestPDF v2026.2.3 works on target Android API level
**Given:** Project targets net10.0-android, SupportedOSPlatformVersion = 21 (Android 5.0)
**Expected:** 
- No runtime errors on Android API 21 (Lollipop)
- No runtime errors on Android API 34 (Android 14)
- Font rendering works correctly across OS versions

---

## MAUI ViewModel & UI Tests (TournamentListViewModel)

### 21. Export Command Binding
**Test:** Export button appears for each tournament in list
**Given:** TournamentListViewModel with 3 tournaments
**Expected:** 
- Export button visible for each tournament box
- Button executes ExportTournamentToPdfCommand with correct tournament ID
- Button disabled while IsBusy = true

---

### 22. Export Success Feedback
**Test:** User receives confirmation after successful export
**Given:** Export completes successfully
**Expected:** 
- Toast or alert shows "PDF exported to [path]"
- Option to "Open PDF" or "Share PDF"
- Export button re-enabled

---

### 23. Export Failure Feedback
**Test:** User receives error message on failure
**Given:** Export fails (e.g., disk full)
**Expected:** 
- Alert shows user-friendly error message
- Export button re-enabled
- User can retry

---

### 24. Loading State During Export
**Test:** Export button shows loading indicator
**Given:** Export in progress (simulated 3-second delay)
**Expected:** 
- Button text changes to "Exporting..." or shows spinner
- Button disabled (CanExecute = false)
- Other tournament buttons remain enabled

---

## Data Integrity

### 25. No Data Mutation
**Test:** Export does not modify tournament data
**Given:** Tournament with 3 contestants
**Expected:** 
- After export, all scores remain unchanged in LocalStorageService
- ContestantDisqualified dictionary unchanged
- ContestantNames dictionary unchanged

---

### 26. Concurrent Export & Edit
**Test:** User edits tournament while export is in progress
**Given:** Export started, user navigates to TournamentDetail and changes scores
**Expected:** 
- Either: Export uses snapshot of data at export start time
- Or: Export reflects new data (eventual consistency)
- No crash or corrupted data

---

## Recommendations for Kaylee (MAUI Implementation)

1. **Service Interface:** Create `IPdfExportService` with method:
   ```csharp
   Task<Result<string>> ExportTournamentToPdfAsync(TournamentDto tournament)
   ```
   Return type should wrap file path + error message.

2. **Error Handling:** Use try/catch around QuestPDF operations with specific error messages for:
   - IOException (disk full, permissions)
   - OutOfMemoryException (too many contestants)
   - Generic fallback error

3. **Debouncing:** Use `CanExecute` on `ExportTournamentToPdfCommand` to check `!IsBusy`.

4. **File Naming:** Use format `Tournament_{Name}_{Date}.pdf`, sanitize name (remove `/`, `:`, etc.)

5. **File Location:** Save to `FileSystem.AppDataDirectory` or ask for user path via file picker if Android 11+ (scoped storage).

6. **User Feedback:** Show toast/alert with file path, offer "Open PDF" action using `Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(path) })`.

7. **Totals Calculation:** For each contestant, iterate `ContestantScores[num]` dictionary and sum all `ObstacleScore.Refusals` and `ObstacleScore.Faults`.

8. **Default Values:** Treat missing dictionary entries as default values (no name = "Contestant N", no disqualified = false, no scores = 0).

9. **Performance:** If tournament has >50 contestants, consider background thread via `Task.Run()` to avoid UI freeze.

10. **Testing:** Since no MAUI test project exists, write manual test checklist based on cases 1-24 above.

---

## Summary

**Total edge cases identified:** 26
**Critical cases (must test):** 1, 2, 5, 8, 13, 16, 22, 23
**Nice-to-have (stress tests):** 11, 18, 20
**Future automation candidates:** 1, 2, 3, 4, 5, 6, 7, 21, 25

**Test infrastructure status:** No MAUI test project exists. All tests will be manual or require new test project setup.

**Next steps:**
1. Kaylee implements `PdfExportService` with QuestPDF
2. Kaylee adds `ExportTournamentToPdfCommand` to `TournamentListViewModel`
3. Simon manually tests critical cases 1, 2, 5, 8, 13, 16, 22, 23
4. Optional: Create MAUI test project for automated ViewModel tests

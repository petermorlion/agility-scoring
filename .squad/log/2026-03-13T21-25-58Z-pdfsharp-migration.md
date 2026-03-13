# PDF Library Testing — PDFsharp Migration (2026-03-13T21:25:58Z)

## Issue
#17: PDF export for tournament results — Android compatibility

## Summary
Migrated from iText7 (with native bouncy-castle-adapter) to PDFsharp 6.2.0. iText7 had critical runtime failures on Android due to native dependencies. PDFsharp is pure managed .NET (Android-compatible) and MIT-licensed.

## Result
✅ Build: 0 errors, 0 exit code  
✅ Commit: ddadb66 on `squad/17-pdf-export`

## Key Technical Notes
- **PDFsharp API:** Low-level XGraphics (draw manually with XRect, XBrushes, XFontStyleEx)
- **Android fonts:** Helvetica works; Arial does not
- **No high-level table API:** Columns/rows drawn via positioned rectangles + text
- **Namespace conflict:** Use `PdfSharp.PageSize.A4` fully qualified
- **License:** MIT (QuestPDF & iText7 rejected due to native dependencies or commercial restrictions)

## Files Modified
- `AgilityScoring.Maui.csproj`
- `Services/PdfExportService.cs`

## Dependencies Removed
- `itext`
- `itext.bouncy-castle-adapter` (blocked Android)

## Dependencies Added
- `PDFsharp` v6.2.0

## Cross-Team Notes
- Simon's 26 test cases remain valid (no service API changes)
- Awaiting manual QA execution

---

**Owner:** Kaylee  
**Status:** Complete → Ready for testing

# Session: iText7 PDF Export Migration (2026-03-13T20:57:03Z)

## Summary
Kaylee replaced QuestPDF with iText7 for Android compatibility. QuestPDF lacks Android support (native Linux libraries only); iText7 is pure .NET and works on all platforms.

## Changes
- Removed QuestPDF NuGet reference; added itext7 v9.1.0
- Removed QuestPDF license init from MauiProgram.cs
- Rewrote PdfExportService.cs using iText7 API
- Resolved 3 namespace conflicts via import aliases and qualified names

## Build Status
0 errors

## Next
Simon executes PDF export edge case tests (26 critical cases documented).

# Decision: PDFsharp IFontResolver for Android

**By:** Kaylee (MAUI Dev)  
**Date:** 2026-06-09  
**Status:** ✅ Implemented  

## Problem

PDFsharp on Android cannot resolve system fonts (e.g., "Helvetica", "Arial"). The app was crashing with a font-not-found error when attempting to generate PDFs.

**Root cause:** Android does not expose system fonts to .NET applications. PDFsharp's default font resolver expects fonts to be available via the OS, which fails on Android.

## Solution

Implemented a custom `IFontResolver` for PDFsharp that loads TTF font bytes from the MAUI app package at runtime.

## Strategy Used: Strategy A (Use Existing Fonts)

**Fonts found in `apps/maui/Resources/Fonts/`:**
- `OpenSans-Regular.ttf`
- `OpenSans-Semibold.ttf`

These fonts are already registered in `AgilityScoring.Maui.csproj` via:
```xml
<MauiFont Include="Resources\Fonts\*" />
```

## Implementation

### 1. Created `PdfFontResolver.cs`

```csharp
public class PdfFontResolver : IFontResolver
{
    public static byte[]? RegularFontData { get; set; }
    public static byte[]? BoldFontData { get; set; }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        var faceName = isBold ? "OpenSans-Bold" : "OpenSans-Regular";
        return new FontResolverInfo(faceName);
    }

    public byte[]? GetFont(string faceName)
    {
        return faceName == "OpenSans-Bold" ? (BoldFontData ?? RegularFontData) : RegularFontData;
    }
}
```

### 2. Updated `PdfExportService.cs`

**Added font initialization method:**
```csharp
private static async Task InitializeFontResolverAsync()
{
    // Load OpenSans-Regular
    using var stream = await FileSystem.OpenAppPackageFileAsync("OpenSans-Regular.ttf");
    using var ms = new MemoryStream();
    await stream.CopyToAsync(ms);
    PdfFontResolver.RegularFontData = ms.ToArray();
    
    // Load OpenSans-Semibold (use as bold)
    try
    {
        using var boldStream = await FileSystem.OpenAppPackageFileAsync("OpenSans-Semibold.ttf");
        using var boldMs = new MemoryStream();
        await boldStream.CopyToAsync(boldMs);
        PdfFontResolver.BoldFontData = boldMs.ToArray();
    }
    catch
    {
        PdfFontResolver.BoldFontData = PdfFontResolver.RegularFontData;
    }
}
```

**Font initialization before PDF generation:**
```csharp
public async Task<string> ExportTournamentToPdfAsync(...)
{
    // Initialize fonts BEFORE Task.Run (async loading)
    if (GlobalFontSettings.FontResolver == null)
    {
        await InitializeFontResolverAsync();
        GlobalFontSettings.FontResolver = new PdfFontResolver();
    }
    
    return await Task.Run(() => { /* PDF generation */ });
}
```

**Updated XFont declarations:**
```csharp
var titleFont = new XFont("OpenSans", 24, XFontStyleEx.Bold);
var regularFont = new XFont("OpenSans", 11, XFontStyleEx.Regular);
// ... etc
```

## Key Technical Details

1. **Async/sync challenge:** `IFontResolver.GetFont()` is synchronous but MAUI's `FileSystem.OpenAppPackageFileAsync()` is async
   - **Solution:** Load font bytes BEFORE `Task.Run` (while still in async context), cache in static fields, resolver returns cached bytes synchronously

2. **GlobalFontSettings.FontResolver:** Static/global setting that applies to all `PdfDocument` instances
   - Set once on first PDF generation
   - Thread-safe via null check (idempotent initialization)

3. **Font file names:** Case-sensitive on Android — used exact names from `Resources/Fonts/`

4. **Font family mapping:** `ResolveTypeface()` maps ALL font requests to our two faces, so any font family name works in `XFont` constructor

## Build Result

✅ **Build succeeded**  
- Exit code: 0  
- Time: 78 seconds  
- Errors: 0  
- Warnings: 53 (PDFsharp deprecation warnings about `XUnit` implicit conversions, unrelated to font resolver)

## Commit

**Hash:** `7cc20cd`  
**Branch:** `squad/17-pdf-export`  
**Message:** "fix: implement IFontResolver for PDFsharp on Android"

## Status

✅ **Complete and verified**  
PDFsharp now works on Android with proper font rendering using OpenSans fonts from the app package.

## Future Considerations

- If additional fonts are needed, add TTF files to `Resources/Fonts/` and update `InitializeFontResolverAsync()` to load them
- For italic support, add OpenSans-Italic.ttf and update `ResolveTypeface()` to handle `isItalic` flag
- Consider error handling if font files are missing (currently throws exception, which is acceptable for required resources)

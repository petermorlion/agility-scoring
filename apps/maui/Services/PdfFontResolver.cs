using PdfSharp.Fonts;

namespace AgilityScoring.Maui.Services;

public class PdfFontResolver : IFontResolver
{
    // Pre-loaded font bytes (set at startup before first PDF generation)
    public static byte[]? RegularFontData { get; set; }
    public static byte[]? BoldFontData { get; set; }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        // Map all font requests to our two faces
        var faceName = isBold ? "OpenSans-Bold" : "OpenSans-Regular";
        return new FontResolverInfo(faceName);
    }

    public byte[]? GetFont(string faceName)
    {
        return faceName == "OpenSans-Bold" ? (BoldFontData ?? RegularFontData) : RegularFontData;
    }
}

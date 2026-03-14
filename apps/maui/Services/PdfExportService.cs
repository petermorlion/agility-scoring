using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;

namespace AgilityScoring.Maui.Services
{
    public class PdfExportService
    {
        private readonly LocalizationService _localizationService;

        public PdfExportService(LocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        private static async Task InitializeFontResolverAsync()
        {
            // Load OpenSans-Regular font bytes from MAUI app package
            using var stream = await FileSystem.OpenAppPackageFileAsync("OpenSans-Regular.ttf");
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            PdfFontResolver.RegularFontData = ms.ToArray();
            
            // Load OpenSans-Semibold (use as bold font)
            try
            {
                using var boldStream = await FileSystem.OpenAppPackageFileAsync("OpenSans-Semibold.ttf");
                using var boldMs = new MemoryStream();
                await boldStream.CopyToAsync(boldMs);
                PdfFontResolver.BoldFontData = boldMs.ToArray();
            }
            catch
            {
                // Fall back to regular font for bold
                PdfFontResolver.BoldFontData = PdfFontResolver.RegularFontData;
            }
        }

        public async Task<string> ExportTournamentToPdfAsync(TournamentDto tournament, List<ContestantResult> contestants)
        {
            // Initialize font resolver on first use (before Task.Run since it's async)
            if (GlobalFontSettings.FontResolver == null)
            {
                await InitializeFontResolverAsync();
                GlobalFontSettings.FontResolver = new PdfFontResolver();
            }

            return await Task.Run(() =>
            {
                var invalidChars = System.IO.Path.GetInvalidFileNameChars();
                var safeName = string.Concat(tournament.Name.Select(c => invalidChars.Contains(c) ? '_' : c));
                var fileName = $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var exportsDir = System.IO.Path.Combine(FileSystem.AppDataDirectory, "exports");
                Directory.CreateDirectory(exportsDir);
                var filePath = System.IO.Path.Combine(exportsDir, fileName);

                var document = new PdfDocument();
                var page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                using var gfx = XGraphics.FromPdfPage(page);

                var titleFont = new XFont("OpenSans", 24, XFontStyleEx.Bold);
                var dateFont = new XFont("OpenSans", 14, XFontStyleEx.Regular);
                var headerFont = new XFont("OpenSans", 11, XFontStyleEx.Bold);
                var regularFont = new XFont("OpenSans", 11, XFontStyleEx.Regular);

                double yPos = 50;
                gfx.DrawString(tournament.Name, titleFont, XBrushes.Black, new XRect(50, yPos, page.Width - 100, 40), XStringFormats.TopLeft);
                yPos += 40;

                gfx.DrawString(tournament.Date, dateFont, XBrushes.Black, new XRect(50, yPos, page.Width - 100, 30), XStringFormats.TopLeft);
                yPos += 50;

                double tableLeft = 50;
                double tableWidth = page.Width - 100;
                double col1 = tableLeft;
                double col2 = col1 + 35;
                double col3 = col2 + 155;
                double col4 = col3 + 65;
                double col5 = col4 + 70;
                double col6 = col5 + 70;
                double rowHeight = 25;

                var pen = new XPen(XColors.Black, 0.5);

                gfx.DrawRectangle(pen, col1, yPos, tableWidth, rowHeight);
                gfx.DrawString(_localizationService["PdfHeaderNumber"], headerFont, XBrushes.Black, new XRect(col1 + 5, yPos + 5, 30, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(_localizationService["PdfHeaderName"], headerFont, XBrushes.Black, new XRect(col2 + 5, yPos + 5, 145, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(_localizationService["PdfHeaderTime"], headerFont, XBrushes.Black, new XRect(col3 + 5, yPos + 5, 55, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(_localizationService["PdfHeaderRefusals"], headerFont, XBrushes.Black, new XRect(col4 + 5, yPos + 5, 60, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(_localizationService["PdfHeaderFaults"], headerFont, XBrushes.Black, new XRect(col5 + 5, yPos + 5, 60, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString(_localizationService["DQAcronym"], headerFont, XBrushes.Black, new XRect(col6 + 5, yPos + 5, 90, rowHeight), XStringFormats.TopLeft);
                yPos += rowHeight;

                foreach (var c in contestants.OrderBy(c => c.ContestantNumber))
                {
                    gfx.DrawRectangle(pen, col1, yPos, tableWidth, rowHeight);

                    gfx.DrawString(c.ContestantNumber.ToString(), regularFont, XBrushes.Black, new XRect(col1 + 5, yPos + 5, 30, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawString(c.Name ?? "", regularFont, XBrushes.Black, new XRect(col2 + 5, yPos + 5, 145, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawString(c.Time ?? "0:00", regularFont, XBrushes.Black, new XRect(col3 + 5, yPos + 5, 55, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawString(c.TotalRefusals.ToString(), regularFont, XBrushes.Black, new XRect(col4 + 5, yPos + 5, 60, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawString(c.TotalFaults.ToString(), regularFont, XBrushes.Black, new XRect(col5 + 5, yPos + 5, 60, rowHeight), XStringFormats.TopLeft);

                    var dqBrush = c.IsDisqualified ? XBrushes.Red : XBrushes.Black;
                    var dqText = c.IsDisqualified ? _localizationService["DQAcronym"] : "";
                    gfx.DrawString(dqText, regularFont, dqBrush, new XRect(col6 + 5, yPos + 5, 90, rowHeight), XStringFormats.TopLeft);

                    yPos += rowHeight;
                }

                document.Save(filePath);

                return filePath;
            });
        }
    }

    public class ContestantResult
    {
        public int ContestantNumber { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public int TotalRefusals { get; set; }
        public int TotalFaults { get; set; }
        public bool IsDisqualified { get; set; }
    }
}

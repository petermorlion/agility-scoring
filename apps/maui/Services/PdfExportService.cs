using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace AgilityScoring.Maui.Services
{
    public class PdfExportService
    {
        public async Task<string> ExportTournamentToPdfAsync(TournamentDto tournament, List<ContestantResult> contestants)
        {
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

                var titleFont = new XFont("Helvetica", 24, XFontStyleEx.Bold);
                var dateFont = new XFont("Helvetica", 14, XFontStyleEx.Regular);
                var headerFont = new XFont("Helvetica", 11, XFontStyleEx.Bold);
                var regularFont = new XFont("Helvetica", 11, XFontStyleEx.Regular);

                double yPos = 50;
                gfx.DrawString(tournament.Name, titleFont, XBrushes.Black, new XRect(50, yPos, page.Width - 100, 40), XStringFormats.TopLeft);
                yPos += 40;

                gfx.DrawString(tournament.Date, dateFont, XBrushes.Black, new XRect(50, yPos, page.Width - 100, 30), XStringFormats.TopLeft);
                yPos += 50;

                double tableLeft = 50;
                double tableWidth = page.Width - 100;
                double col1 = tableLeft;
                double col2 = col1 + 40;
                double col3 = col2 + 200;
                double col4 = col3 + 80;
                double col5 = col4 + 80;
                double rowHeight = 25;

                var pen = new XPen(XColors.Black, 0.5);

                gfx.DrawRectangle(pen, col1, yPos, tableWidth, rowHeight);
                gfx.DrawString("#", headerFont, XBrushes.Black, new XRect(col1 + 5, yPos + 5, 30, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString("Name", headerFont, XBrushes.Black, new XRect(col2 + 5, yPos + 5, 190, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString("Refusals", headerFont, XBrushes.Black, new XRect(col3 + 5, yPos + 5, 70, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString("Faults", headerFont, XBrushes.Black, new XRect(col4 + 5, yPos + 5, 70, rowHeight), XStringFormats.TopLeft);
                gfx.DrawString("DQ", headerFont, XBrushes.Black, new XRect(col5 + 5, yPos + 5, 70, rowHeight), XStringFormats.TopLeft);
                yPos += rowHeight;

                foreach (var c in contestants.OrderBy(c => c.ContestantNumber))
                {
                    gfx.DrawRectangle(pen, col1, yPos, tableWidth, rowHeight);

                    gfx.DrawString(c.ContestantNumber.ToString(), regularFont, XBrushes.Black, new XRect(col1 + 5, yPos + 5, 30, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawString(c.Name ?? "", regularFont, XBrushes.Black, new XRect(col2 + 5, yPos + 5, 190, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawString(c.TotalRefusals.ToString(), regularFont, XBrushes.Black, new XRect(col3 + 5, yPos + 5, 70, rowHeight), XStringFormats.TopLeft);
                    gfx.DrawString(c.TotalFaults.ToString(), regularFont, XBrushes.Black, new XRect(col4 + 5, yPos + 5, 70, rowHeight), XStringFormats.TopLeft);

                    var dqBrush = c.IsDisqualified ? XBrushes.Red : XBrushes.Black;
                    var dqText = c.IsDisqualified ? "DQ" : "";
                    gfx.DrawString(dqText, regularFont, dqBrush, new XRect(col5 + 5, yPos + 5, 70, rowHeight), XStringFormats.TopLeft);

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
        public int TotalRefusals { get; set; }
        public int TotalFaults { get; set; }
        public bool IsDisqualified { get; set; }
    }
}

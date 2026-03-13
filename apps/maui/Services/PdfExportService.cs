using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iTextCell = iText.Layout.Element.Cell;

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

                using var writer = new PdfWriter(filePath);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
                document.SetMargins(50, 50, 50, 50);

                document.Add(new Paragraph(tournament.Name).SetFontSize(24).SimulateBold());
                document.Add(new Paragraph(tournament.Date).SetFontSize(14));

                var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 4, 2, 2, 1 }))
                    .UseAllAvailableWidth()
                    .SetMarginTop(20);

                foreach (var header in new[] { "#", "Name", "Refusals", "Faults", "DQ" })
                    table.AddHeaderCell(new iTextCell().Add(new Paragraph(header).SimulateBold()));

                foreach (var c in contestants.OrderBy(c => c.ContestantNumber))
                {
                    table.AddCell(c.ContestantNumber.ToString());
                    table.AddCell(c.Name ?? "");
                    table.AddCell(c.TotalRefusals.ToString());
                    table.AddCell(c.TotalFaults.ToString());
                    var dqPara = new Paragraph(c.IsDisqualified ? "DQ" : "")
                        .SetFontColor(c.IsDisqualified ? ColorConstants.RED : ColorConstants.BLACK);
                    table.AddCell(new iTextCell().Add(dqPara));
                }

                document.Add(table);

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

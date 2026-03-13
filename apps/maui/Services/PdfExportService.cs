using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AgilityScoring.Maui.Services
{
    public class PdfExportService
    {
        public async Task<string> ExportTournamentToPdfAsync(TournamentDto tournament, List<ContestantResult> contestants)
        {
            return await Task.Run(() =>
            {
                var invalidChars = Path.GetInvalidFileNameChars();
                var safeName = string.Concat(tournament.Name.Select(c => invalidChars.Contains(c) ? '_' : c));
                var fileName = $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var exportsDir = Path.Combine(FileSystem.AppDataDirectory, "exports");
                Directory.CreateDirectory(exportsDir);
                var filePath = Path.Combine(exportsDir, fileName);

                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.Content().Column(col =>
                        {
                            col.Item().Text(tournament.Name).FontSize(24).Bold();
                            col.Item().Text(tournament.Date).FontSize(16);
                            col.Item().PaddingTop(20).Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.ConstantColumn(50);   // #
                                    cols.RelativeColumn();      // Name
                                    cols.ConstantColumn(80);    // Refusals
                                    cols.ConstantColumn(80);    // Faults
                                    cols.ConstantColumn(60);    // DQ
                                });

                                // Header row
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("#").Bold();
                                    header.Cell().Element(CellStyle).Text("Name").Bold();
                                    header.Cell().Element(CellStyle).Text("Refusals").Bold();
                                    header.Cell().Element(CellStyle).Text("Faults").Bold();
                                    header.Cell().Element(CellStyle).Text("DQ").Bold();
                                });

                                // Data rows
                                foreach (var contestant in contestants.OrderBy(c => c.ContestantNumber))
                                {
                                    table.Cell().Element(CellStyle).Text(contestant.ContestantNumber.ToString());
                                    table.Cell().Element(CellStyle).Text(contestant.Name ?? "");
                                    table.Cell().Element(CellStyle).Text(contestant.TotalRefusals.ToString());
                                    table.Cell().Element(CellStyle).Text(contestant.TotalFaults.ToString());
                                    table.Cell().Element(CellStyle).Text(contestant.IsDisqualified ? "DQ" : "")
                                        .FontColor(contestant.IsDisqualified ? QuestPDF.Helpers.Colors.Red.Medium : QuestPDF.Helpers.Colors.Black);
                                }
                            });
                        });
                    });
                }).GeneratePdf(filePath);

                return filePath;
            });
        }

        private static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
        {
            return container.BorderBottom(1).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).PaddingVertical(5);
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

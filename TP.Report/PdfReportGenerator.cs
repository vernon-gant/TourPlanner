using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using TP.Report.ReportTypes;

namespace TP.Report;

public class PdfReportGenerator : ReportGenerator
{
    private PdfWriter _writer = null!;

    private PdfDocument _entryDocument = null!;

    public override void Init()
    {
        _writer = new PdfWriter(_filePath);
        _entryDocument = new PdfDocument(_writer);
    }

    public override void Generate(SingleTourReport report)
    {
        using Document document = new(_entryDocument);
        document.Add(new Paragraph("Tour Report").SetFontSize(20)
                                                 .SetBold()
                                                 .SetTextAlignment(TextAlignment.CENTER)
                                                 .SetMarginTop(20)
                                                 .SetMarginBottom(20));
        Table table = new Table(UnitValue.CreatePercentArray([1, 2])).UseAllAvailableWidth()
                                                                     .SetMarginBottom(20);

        AddTableRow(table, "ID:", report.TourId);
        AddTableRow(table, "Tour Name:", report.TourName);
        AddTableRow(table, "Description:", report.TourDescription);
        AddTableRow(table, "Start Location:", report.StartLocation);
        AddTableRow(table, "End Location:", report.EndLocation);
        AddTableRow(table, "Distance:", report.Distance);
        AddTableRow(table, "Estimated Time:", report.EstimatedTime);
        AddTableRow(table, "Transport Type:", report.TransportType);
        AddTableRow(table, "Popularity:", report.Popularity);
        AddTableRow(table, "Child Friendliness:", report.ChildFriendly);
        AddTableRow(table, "Created At:", report.CreatedAt);

        document.Add(table);

        if (report.TourLogs.Count <= 0) return;

        document.Add(new Paragraph("Tour Logs").SetFontSize(18)
                                               .SetBold()
                                               .SetTextAlignment(TextAlignment.CENTER)
                                               .SetMarginTop(20)
                                               .SetMarginBottom(20));

        foreach (TourLogReportModel log in report.TourLogs)
        {
            Table tourLogTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2 })).UseAllAvailableWidth()
                                                                                              .SetMarginBottom(20);

            AddTableRow(tourLogTable, "Log Comment:", log.Comment);
            AddTableRow(tourLogTable, "Difficulty:", log.Difficulty);
            AddTableRow(tourLogTable, "Total Distance:", log.Distance);
            AddTableRow(tourLogTable, "Total Time:", log.TotalTime);
            AddTableRow(tourLogTable, "Rating:", log.Rating);
            AddTableRow(tourLogTable, "Created At:", log.CreatedAt);

            document.Add(tourLogTable);
        }

        _entryDocument.Close();
        _writer.Close();
    }

    public override void Generate(TourSummaryReport report)
    {
        using Document document = new(_entryDocument);
        document.Add(new Paragraph("Tour Summary Report").SetFontSize(20)
                                                         .SetBold()
                                                         .SetTextAlignment(TextAlignment.CENTER)
                                                         .SetMarginTop(20)
                                                         .SetMarginBottom(20));

        foreach (TourSummary summary in report.TourSummaries)
        {
            Table summaryTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2 })).UseAllAvailableWidth()
                                                                                              .SetMarginBottom(20);

            AddTableRow(summaryTable, "Tour ID:", summary.TourId);
            AddTableRow(summaryTable, "Tour Name:", summary.TourName);
            AddTableRow(summaryTable, "Average Distance:", summary.AverageDistance);
            AddTableRow(summaryTable, "Average Time:", summary.AverageTime);
            AddTableRow(summaryTable, "Average Rating:", summary.AverageRating);

            document.Add(summaryTable);
        }

        _entryDocument.Close();
        _writer.Close();
    }

    private void AddTableRow(Table table, string propertyName, string value)
    {
        Cell propertyCell = new Cell().Add(new Paragraph(propertyName).SetBold());
        Cell valueCell = new Cell().Add(new Paragraph(value));
        table.AddCell(propertyCell);
        table.AddCell(valueCell);
    }
}
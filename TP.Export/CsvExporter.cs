using Microsoft.Extensions.Logging;
using TP.Utils;

namespace TP.Export;

public class CsvExporter(TourMapper tourMapper, ILogger<CsvExporter> logger) : TourExporter(tourMapper, logger)
{
    private StreamWriter _streamWriter = null!;

    private readonly string _tourHeaders = string.Join(',', ExportHeaders.TourHeaders);

    private readonly string _tourLogHeaders = string.Join(',', ExportHeaders.TourLogHeaders);

    public override bool CanHandle(string format) => format == "csv";

    protected override string GetContentType() => ContentTypes.Csv;

    protected override void WriteTours(List<TourExportModel> tourExportModels)
    {
        _streamWriter = new StreamWriter(_tempExportFilePath);
        _streamWriter.WriteLine(_tourHeaders);
        foreach (var tour in tourExportModels)
        {
            _streamWriter.WriteLine($"{tour.TourNumber},'{tour.Description}','{tour.Name}',{tour.TransportType},'{tour.Start}',{tour.StartLatitude},{tour.StartLongitude},'{tour.End}',{tour.EndLatitude},{tour.EndLongitude},{tour.RouteGeometry},{tour.DistanceMeters},{tour.EstimatedTime},{tour.Popularity},{tour.ChildFriendliness}");
        }
    }

    protected override void WriteTourLogs(List<TourLogExportModel> tourLogs)
    {
        _streamWriter.WriteLine();
        _streamWriter.WriteLine(_tourLogHeaders);
        foreach (var log in tourLogs)
        {
            _streamWriter.WriteLine($"{log.TourNumber},'{log.Comment}',{log.Difficulty},{log.TotalDistanceMeters},{log.TotalTime},{log.Rating}");
        }
    }

    protected override void Save()
    {
        _streamWriter.Flush();
        _streamWriter.Close();
    }
}
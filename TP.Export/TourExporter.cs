using Microsoft.Extensions.Logging;
using TP.Domain;
using TP.Utils;

namespace TP.Export;

public abstract class TourExporter(TourMapper tourMapper, ILogger<TourExporter> logger)
{
    protected string _tempExportFilePath = string.Empty;
    public OperationResult<ExportResult> ExportTours(List<Tour> tours, bool withTourLogs)
    {
        try
        {
            _tempExportFilePath = Path.GetTempFileName();
            List<TourExportModel> tourExportModels = tourMapper.MapTours(tours, withTourLogs);

            WriteTours(tourExportModels);

            if (withTourLogs && AnyTourLogs(tourExportModels)) WriteTourLogs(tourMapper.MapTourLogs(tourExportModels));

            Save();

            return OperationResult<ExportResult>.Ok(new ExportResult(new FileStream(_tempExportFilePath, FileMode.Open, FileAccess.Read, FileShare.Read), GetContentType()));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during tour export");
            return OperationResult<ExportResult>.Error();
        }
    }
    public abstract bool CanHandle(string format);

    protected bool AnyTourLogs(List<TourExportModel> tourExportModels) => tourExportModels.Any(tourExportModel => tourExportModel.TourLogs.Count != 0);

    protected abstract string GetContentType();

    protected abstract void WriteTours(List<TourExportModel> tourExportModels);

    protected abstract void WriteTourLogs(List<TourLogExportModel> tourLogs);

    protected abstract void Save();
}
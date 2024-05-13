using AutoMapper;
using Microsoft.Extensions.Logging;
using TP.Domain;
using TP.Utils;
using Mapper = Npoi.Mapper.Mapper;

namespace TP.Export;

public class XlsxExporter(IMapper mapper, ILogger<XlsxExporter> logger) : TourExporter
{
    public OperationResult<ExportResult> ExportTours(List<Tour> tours, bool withTourLogs)
    {
        try
        {
            string tempExportFilePath = Path.GetTempFileName().Replace(".tmp",".xlsx");
            Mapper excelMapper = new Mapper();
            List<TourExportModel> tourExportModels = ToursToExportModels(tours, withTourLogs);

            if (withTourLogs && !AnyTourLogs(tourExportModels)) return OperationResult<ExportResult>.Error();

            excelMapper.Put(tourExportModels, "Tours");

            if (withTourLogs) excelMapper.Put(TourLogsToExportModels(tourExportModels), "TourLogs");

            excelMapper.Save(tempExportFilePath, false);

            return OperationResult<ExportResult>.Ok(new ExportResult(new FileStream(tempExportFilePath, FileMode.Open, FileAccess.Read, FileShare.Read),ContentTypes.Xlsx));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during tour export");
            return OperationResult<ExportResult>.Error();
        }
    }

    private List<TourExportModel> ToursToExportModels(List<Tour> tours, bool withTourLogs) => tours
        .Select((tour, idx) => mapper.Map<TourExportModel>(tour,
            context => { context.AfterMap((_, exportModel) => exportModel.TourNumber = idx); }))
        .Select(tourExportModel =>
        {
            if (withTourLogs) return tourExportModel;
            tourExportModel.Popularity = null;
            tourExportModel.ChildFriendliness = null;
            return tourExportModel;
        })
        .ToList();

    private List<TourLogExportModel> TourLogsToExportModels(List<TourExportModel> tourExportModels) => tourExportModels
        .SelectMany(tourExportModel => mapper.Map<List<TourLogExportModel>>(tourExportModel.TourLogs).Select(
            tourLogExportModel =>
            {
                tourLogExportModel.TourNumber = tourExportModel.TourNumber;
                return tourLogExportModel;
            })).ToList();

    public bool CanHandle(string format) => format == ExportImportFileFormats.XlsxFormat;

    private bool AnyTourLogs(List<TourExportModel> tourExportModels) => tourExportModels.Any(tourExportModel => tourExportModel.TourLogs.Count != 0);
}
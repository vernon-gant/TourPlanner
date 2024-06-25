using Microsoft.Extensions.Logging;
using TP.Utils;
using Mapper = Npoi.Mapper.Mapper;

namespace TP.Export;

public class XlsxExporter(TourMapper tourMapper, ILogger<XlsxExporter> logger) : TourExporter(tourMapper, logger)
{
    private readonly Mapper _excelMapper = new();

    public override bool CanHandle(string format) => format == "xlsx";

    protected override string GetContentType() => ContentTypes.Xlsx;

    protected override void WriteTourExportModels(List<TourExportModel> tourExportModels)
    {
        _excelMapper.Put(tourExportModels, "Tours");
    }

    protected override void WriteTourLogs(List<TourLogExportModel> tourLogs)
    {
        _excelMapper.Put(tourLogs, "TourLogs");
    }

    protected override void Save()
    {
        _excelMapper.Save(_tempExportFilePath, false);
    }
}
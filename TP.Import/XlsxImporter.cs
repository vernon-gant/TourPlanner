using AutoMapper;
using ExcelDataReader;
using Microsoft.Extensions.Logging;
using TP.Export;
using TP.Utils;

namespace TP.Import;

public class XlsxImporter(IMapper mapper, ILogger<XlsxImporter> logger) : TourImporter(mapper)
{
    private IExcelDataReader _excelDataReader = null!;

    protected override OperationResult<List<TourExportModel>> ReadTours()
    {
        try
        {
            _excelDataReader = ExcelReaderFactory.CreateReader(_fileStream);

            _excelDataReader.Read();

            if (!ValidTourHeaders()) return OperationResult<List<TourExportModel>>.Error();

            List<TourExportModel> tourExportModels = new();
            while (_excelDataReader.Read())
            {
                tourExportModels.Add(new TourExportModel
                                     {
                                         TourNumber = (int)_excelDataReader.GetDouble(0),
                                         Description = _excelDataReader.GetString(1),
                                         Name = _excelDataReader.GetString(2),
                                         TransportType = GetTransportType(_excelDataReader.GetString(3)),
                                         Start = _excelDataReader.GetString(4),
                                         StartLatitude = (decimal)_excelDataReader.GetDouble(5),
                                         StartLongitude = (decimal)_excelDataReader.GetDouble(6),
                                         End = _excelDataReader.GetString(7),
                                         EndLatitude = (decimal)_excelDataReader.GetDouble(8),
                                         EndLongitude = (decimal)_excelDataReader.GetDouble(9),
                                         RouteGeometry = _excelDataReader.GetString(10),
                                         DistanceMeters = (decimal)_excelDataReader.GetDouble(11),
                                         EstimatedTime = (long)_excelDataReader.GetDouble(12),
                                         Popularity = GetPopularity(_excelDataReader.GetString(13)),
                                         ChildFriendliness = _excelDataReader.GetString(14)
                                     });
            }

            return OperationResult<List<TourExportModel>>.Ok(tourExportModels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during tour import");
            return OperationResult<List<TourExportModel>>.Error();
        }
    }

    protected override OperationResult<List<TourLogExportModel>> ReadTourLogs()
    {
        try
        {
            _excelDataReader.NextResult();

            _excelDataReader.Read();

            List<TourLogExportModel> tourLogExportModels = new();
            while (_excelDataReader.Read())
            {
                tourLogExportModels.Add(new TourLogExportModel
                                        {
                                            TourNumber = (int)_excelDataReader.GetDouble(0),
                                            Comment = _excelDataReader.GetString(1),
                                            Difficulty = GetDifficulty(_excelDataReader.GetString(2)),
                                            TotalDistanceMeters = (decimal)_excelDataReader.GetDouble(3),
                                            TotalTime = (long)_excelDataReader.GetDouble(4),
                                            Rating = (short)_excelDataReader.GetDouble(5)
                                        });
            }

            return OperationResult<List<TourLogExportModel>>.Ok(tourLogExportModels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during tour log import");
            return OperationResult<List<TourLogExportModel>>.Error();
        }
    }

    protected override bool ValidTourHeaders() => Enumerable.Range(0, ExportHeaders.TourHeaders.Count)
                                                            .Select(_excelDataReader.GetString)
                                                            .SequenceEqual(ExportHeaders.TourHeaders);

    protected override bool ValidTourLogHeaders() => Enumerable.Range(0, ExportHeaders.TourHeaders.Count)
                                                               .Select(_excelDataReader.GetString)
                                                               .SequenceEqual(ExportHeaders.TourHeaders);

    public override bool CanHandle(string format) => format == ExportImportFileFormats.XlsxFormat;
}
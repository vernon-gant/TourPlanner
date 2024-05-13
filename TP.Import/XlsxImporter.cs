using AutoMapper;
using ExcelDataReader;
using Microsoft.Extensions.Logging;
using TP.Domain;
using TP.Export;
using TP.Utils;

namespace TP.Import;

public class XlsxImporter(IMapper mapper, ILogger<XlsxImporter> logger) : TourImporter
{
    private static readonly List<string> Headers =
    [
        "TourNumber", "Description", "Name", "TransportType", "Start", "StartLatitude", "StartLongitude", "End",
        "EndLatitude", "EndLongitude", "DistanceMeters", "EstimatedTime", "Popularity", "ChildFriendliness"
    ];

    public OperationResult<List<Tour>> Import(Stream fileStream)
    {
        try
        {
            IExcelDataReader excelDataReader = ExcelReaderFactory.CreateReader(fileStream);

            excelDataReader.Read();

            if (!ValidHeaders(excelDataReader)) return OperationResult<List<Tour>>.Error();

            List<TourExportModel> importedTours = ReadTours(excelDataReader);

            excelDataReader.NextResult();

            excelDataReader.Read();

            List<TourLogExportModel> importedTourLogs = ReadTourLogs(excelDataReader);

            List<Tour> tours = importedTours.Select(tourExportModel =>
            {
                tourExportModel.TourLogs.AddRange(importedTourLogs
                    .Where(tourLogImportInfo => tourLogImportInfo.TourNumber == tourExportModel.TourNumber)
                    .Select(mapper.Map<TourLog>)
                    .ToList());
                return tourExportModel;
            }).Select(mapper.Map<Tour>).ToList();

            return OperationResult<List<Tour>>.Ok(tours);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during import");
            return OperationResult<List<Tour>>.Error();
        }
    }

    private List<TourExportModel> ReadTours(IExcelDataReader excelDataReader)
    {
        List<TourExportModel> tourExportModels = new();
        while (excelDataReader.Read())
        {
            tourExportModels.Add(new TourExportModel()
            {
                TourNumber = (int)excelDataReader.GetDouble(0),
                Description = excelDataReader.GetString(1),
                Name = excelDataReader.GetString(2),
                TransportType = GetTransportType(excelDataReader.GetString(3)),
                Start = excelDataReader.GetString(4),
                StartLatitude = (decimal)excelDataReader.GetDouble(5),
                StartLongitude = (decimal)excelDataReader.GetDouble(6),
                End = excelDataReader.GetString(7),
                EndLatitude = (decimal)excelDataReader.GetDouble(8),
                EndLongitude = (decimal)excelDataReader.GetDouble(9),
                DistanceMeters = (decimal)excelDataReader.GetDouble(10),
                EstimatedTime = (long)excelDataReader.GetDouble(11),
                Popularity = GetPopularity(excelDataReader.GetString(12)),
                ChildFriendliness = excelDataReader.GetString(13)
            });
        }

        return tourExportModels;
    }

    private static TransportType GetTransportType(string transportType) => transportType switch
    {
        "Foot" => TransportType.Foot,
        "Car" => TransportType.Car,
        "Truck" => TransportType.Truck,
        "Bicycle" => TransportType.Bicycle,
        "Bike" => TransportType.Bike,
        _ => throw new ArgumentOutOfRangeException(nameof(transportType), transportType, null)
    };

    private static Popularity? GetPopularity(string? transportType) => transportType switch
    {
        "NotPopular" => Popularity.Popular,
        "SlightlyPopular" => Popularity.SlightlyPopular,
        "ModeratelyPopular" => Popularity.ModeratelyPopular,
        "Popular" => Popularity.Popular,
        "HighlyPopular" => Popularity.HighlyPopular,
        "ExtremelyPopular" => Popularity.ExtremelyPopular,
        _ => null
    };

    private bool ValidHeaders(IExcelDataReader excelDataReader) => Enumerable.Range(0, Headers.Count)
        .Select(excelDataReader.GetString).SequenceEqual(Headers);

    private List<TourLogExportModel> ReadTourLogs(IExcelDataReader excelDataReader)
    {
        List<TourLogExportModel> tourLogExportModels = new();
        while (excelDataReader.Read())
        {
            tourLogExportModels.Add(new TourLogExportModel
            {
                TourNumber = (int)excelDataReader.GetDouble(0),
                Comment = excelDataReader.GetString(2),
                Difficulty = GetDifficulty(excelDataReader.GetString(3)),
                TotalDistanceMeters = (decimal)excelDataReader.GetDouble(4),
                TotalTime = (long)excelDataReader.GetDouble(5),
                Rating = (short)excelDataReader.GetDouble(6)
            });
        }

        return tourLogExportModels;
    }

    private Difficulty GetDifficulty(string difficulty) => difficulty switch
    {
        "VeryHard" => Difficulty.VeryHard,
        "Hard" => Difficulty.Hard,
        "Normal" => Difficulty.Normal,
        "Easy" => Difficulty.Easy,
        "VeryEasy" => Difficulty.VeryEasy,
        _ => throw new ArgumentException()
    };

    public bool CanHandle(string format) => format == ExportImportFileFormats.XlsxFormat;
}
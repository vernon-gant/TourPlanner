using AutoMapper;
using TP.Domain;
using TP.Export;
using TP.Utils;

namespace TP.Import;

public abstract class TourImporter(IMapper mapper)
{
    protected Stream _fileStream = null!;

    public OperationResult<List<Tour>> Import(Stream fileStream)
    {
        _fileStream = fileStream;

        OperationResult<List<TourExportModel>> toursResult = ReadTours();

        if (!toursResult.IsOk) return OperationResult<List<Tour>>.Error();

        OperationResult<List<TourLogExportModel>> tourLogsResult = ReadTourLogs();

        return !tourLogsResult.IsOk ? OperationResult<List<Tour>>.Error() : OperationResult<List<Tour>>.Ok(MergeResults(toursResult.Result!, tourLogsResult.Result!));
    }

    protected abstract OperationResult<List<TourExportModel>> ReadTours();

    protected abstract OperationResult<List<TourLogExportModel>> ReadTourLogs();

    protected abstract bool ValidTourHeaders();

    protected abstract bool ValidTourLogHeaders();

    public abstract bool CanHandle(string format);

    protected Difficulty GetDifficulty(string difficulty) => difficulty switch
    {
        "VeryHard" => Difficulty.VeryHard,
        "Hard" => Difficulty.Hard,
        "Normal" => Difficulty.Normal,
        "Easy" => Difficulty.Easy,
        "VeryEasy" => Difficulty.VeryEasy,
        _ => throw new ArgumentException()
    };

    protected static TransportType GetTransportType(string transportType) => transportType switch
    {
        "Foot" => TransportType.Foot,
        "Car" => TransportType.Car,
        "Truck" => TransportType.Truck,
        "Bicycle" => TransportType.Bicycle,
        "Bike" => TransportType.Bike,
        _ => throw new ArgumentOutOfRangeException(nameof(transportType), transportType, null)
    };

    protected static Popularity? GetPopularity(string? transportType) => transportType switch
    {
        "NotPopular" => Popularity.Popular,
        "SlightlyPopular" => Popularity.SlightlyPopular,
        "ModeratelyPopular" => Popularity.ModeratelyPopular,
        "Popular" => Popularity.Popular,
        "HighlyPopular" => Popularity.HighlyPopular,
        "ExtremelyPopular" => Popularity.ExtremelyPopular,
        _ => null
    };

    private List<Tour> MergeResults(List<TourExportModel> importedTours, List<TourLogExportModel> importedTourLogs)
    {
        List<Tour> tours = importedTours.Select(tourExportModel =>
        {
            tourExportModel.TourLogs.AddRange(importedTourLogs
                                             .Where(tourLogImportInfo => tourLogImportInfo.TourNumber == tourExportModel.TourNumber)
                                             .Select(mapper.Map<TourLog>)
                                             .ToList());
            return tourExportModel;
        }).Select(mapper.Map<Tour>).ToList();
        return tours;
    }
}
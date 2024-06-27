using System.Text;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TP.Export;
using TP.Utils;

namespace TP.Import;

public class CsvImporter(IMapper mapper, ILogger<CsvImporter> logger) : TourImporter(mapper)
{
    private StreamReader _streamReader = null!;

    protected override OperationResult<List<TourExportModel>> ReadTours()
    {
        try
        {
            _streamReader = new StreamReader(_fileStream);

            if (!ValidTourHeaders()) return OperationResult<List<TourExportModel>>.Error();

            List<TourExportModel> tourExportModels = new();

            while (_streamReader.ReadLine() is { } line && !string.IsNullOrWhiteSpace(line))
            {
                var values = ParseCsvLine(line);
                if (values.Count != ExportHeaders.TourHeaders.Count)
                {
                    logger.LogWarning("Invalid number of columns in line: {Line}", line);
                    continue;
                }

                var tourExportModel = new TourExportModel
                {
                    TourNumber = int.Parse(values[0]),
                    Description = values[1].Trim('"'),
                    Name = values[2].Trim('"'),
                    TransportType = GetTransportType(values[3]),
                    Start = values[4].Trim('"'),
                    StartLatitude = decimal.Parse(values[5]),
                    StartLongitude = decimal.Parse(values[6]),
                    End = values[7].Trim('"'),
                    EndLatitude = decimal.Parse(values[8]),
                    EndLongitude = decimal.Parse(values[9]),
                    RouteGeometry = values[10],
                    DistanceMeters = decimal.Parse(values[11]),
                    EstimatedTime = long.Parse(values[12]),
                    Popularity = GetPopularity(values[13]),
                    ChildFriendliness = values[14]
                };
                tourExportModels.Add(tourExportModel);
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
            if (_streamReader.EndOfStream) return OperationResult<List<TourLogExportModel>>.Ok(new List<TourLogExportModel>());

            List<TourLogExportModel> tourLogExportModels = new();

            if (!ValidTourLogHeaders()) return OperationResult<List<TourLogExportModel>>.Error();

            while (_streamReader.ReadLine() is { } line && !string.IsNullOrWhiteSpace(line))
            {
                var values = ParseCsvLine(line);
                if (values.Count != ExportHeaders.TourLogHeaders.Count)
                {
                    logger.LogWarning("Invalid number of columns in line: {Line}", line);
                    continue;
                }

                var tourLogExportModel = new TourLogExportModel
                {
                    TourNumber = int.Parse(values[0]),
                    Comment = values[1].Trim('"'),
                    Difficulty = GetDifficulty(values[2]),
                    TotalDistanceMeters = decimal.Parse(values[3]),
                    TotalTime = long.Parse(values[4]),
                    Rating = short.Parse(values[5])
                };
                tourLogExportModels.Add(tourLogExportModel);
            }

            return OperationResult<List<TourLogExportModel>>.Ok(tourLogExportModels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during tour log import");
            return OperationResult<List<TourLogExportModel>>.Error();
        }
    }

    protected override bool ValidTourHeaders()
    {
        var tourHeader = _streamReader.ReadLine();
        return tourHeader != null && tourHeader.Split(',')
                                               .SequenceEqual(ExportHeaders.TourHeaders, StringComparer.InvariantCultureIgnoreCase);
    }

    protected override bool ValidTourLogHeaders()
    {
        var logHeader = _streamReader.ReadLine();
        return logHeader != null && logHeader.Split(',')
                                             .SequenceEqual(ExportHeaders.TourLogHeaders, StringComparer.InvariantCultureIgnoreCase);
    }

    public override bool CanHandle(string format) => format == ExportImportFileFormats.CsvFormat;

    private List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();
        var inQuotes = false;
        var currentValue = new StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i < line.Length - 1 && line[i + 1] == '"')
                {
                    // If inside quotes and next character is also a quote, it's an escaped quote
                    currentValue.Append(c);
                    i++; // Skip next character
                }
                else
                {
                    inQuotes = !inQuotes; // Toggle inQuotes state
                }
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(currentValue.ToString());
                currentValue.Clear();
            }
            else
            {
                currentValue.Append(c);
            }
        }

        values.Add(currentValue.ToString());
        return values;
    }
}

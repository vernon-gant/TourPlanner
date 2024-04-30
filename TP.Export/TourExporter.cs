using TP.Domain;
using TP.Utils;

namespace TP.Export;

public interface TourExporter
{
    OperationResult<ExportResult> ExportTours(List<Tour> tours, bool withTourLogs);

    bool CanHandle(string format);
}
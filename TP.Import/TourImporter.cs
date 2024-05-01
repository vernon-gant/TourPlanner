using TP.Domain;
using TP.Utils;

namespace TP.Import;

public interface TourImporter
{
    OperationResult<List<Tour>> Import(Stream fileStream);

    bool CanHandle(string format);
}
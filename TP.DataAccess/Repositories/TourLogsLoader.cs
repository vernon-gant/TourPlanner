using TP.Domain;

namespace TP.DataAccess.Repositories;

public interface TourLoader
{
    ValueTask LoadTourLogsAsync(Tour tour);

    ValueTask LoadTourLogsAsync(List<Tour> tours);
}
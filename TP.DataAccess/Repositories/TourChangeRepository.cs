using TP.Domain;

namespace TP.DataAccess.Repositories;

public interface TourChangeRepository
{
    ValueTask<Tour> CreateTourAsync(Tour tour);

    ValueTask<List<Tour>> CreateRangeAsync(List<Tour> tours);

    ValueTask<Tour> UpdateTourAsync(Tour tour);

    ValueTask DeleteTourAsync(Guid id);
}
using TP.Domain;

namespace TP.DataAccess.Repositories;

public interface TourChangeRepository
{
    ValueTask<Tour> CreateTourAsync(Tour tour);

    ValueTask<Tour> UpdateTourAsync(Tour tour);

    ValueTask DeleteTourAsync(Guid id);
}
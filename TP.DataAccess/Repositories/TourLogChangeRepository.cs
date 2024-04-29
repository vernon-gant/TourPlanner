using TP.Domain;

namespace TP.DataAccess.Repositories;

public interface TourLogChangeRepository
{
    ValueTask<TourLog> CreateTourLogAsync(TourLog tourLog);

    ValueTask UpdateTourLogAsync(TourLog tourLog);

    ValueTask DeleteTourLogAsync(Guid id);
}
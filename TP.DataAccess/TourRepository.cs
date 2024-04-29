using TP.Domain;

namespace TP.DataAccess;

public interface TourRepository
{
    ValueTask<List<Tour>> GetAllToursAsync();

    ValueTask<Tour?> GetTourByIdAsync(Guid id);

    IQueryable<Tour> GetAllToursQueryable();

    IQueryable<Tour> GetTourByIdQueryable(Guid id);

    ValueTask<Tour> AddTourAsync(Tour tour);

    ValueTask<Tour> UpdateTourAsync(Tour tour);

    ValueTask DeleteTourAsync(Guid id);
}
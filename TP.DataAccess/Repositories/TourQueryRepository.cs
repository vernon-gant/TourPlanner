using TP.Domain;

namespace TP.DataAccess;

public interface TourQueryRepository
{
    ValueTask<List<Tour>> GetAllToursAsync();

    ValueTask<Tour?> GetTourByIdAsync(Guid id);

    IQueryable<Tour> GetAllToursQueryable();

    IQueryable<Tour> GetTourByIdQueryable(Guid id);
}
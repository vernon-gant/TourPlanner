using TP.Domain;

namespace TP.DataAccess.Repositories;

public interface TourQueryRepository
{
    ValueTask<List<Tour>> GetAllToursAsync();

    ValueTask<Tour?> GetTourByIdAsync(Guid id);

    ValueTask<List<Tour>> GetByIds(HashSet<Guid> tourIds, bool withTours);

    IQueryable<Tour> GetAllToursQueryable();

    IQueryable<Tour> GetTourByIdQueryable(Guid id);
}
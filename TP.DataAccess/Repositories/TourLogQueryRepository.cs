using TP.Domain;

namespace TP.DataAccess;

public interface TourLogQueryRepository
{
    ValueTask<List<TourLog>> GetAllToursAsync();

    ValueTask<TourLog?> GetTourByIdAsync(Guid tourLogId);

    IQueryable<TourLog> GetAllToursQueryable();

    IQueryable<TourLog> GetTourByIdQueryable(Guid tourLogId);
}
using TP.Domain;

namespace TP.DataAccess.Repositories;

public interface TourLogQueryRepository
{
    ValueTask<List<TourLog>> GetAllTourLogsAsync();

    ValueTask<TourLog?> GetTourLogByIdAsync(Guid tourLogId);

    IQueryable<TourLog> GetAllTourLogsQueryable();

    IQueryable<TourLog> GetTourLogByIdQueryable(Guid tourLogId);
}